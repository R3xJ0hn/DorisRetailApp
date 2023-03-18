CREATE PROCEDURE [dbo].[spCategoryGetSummaryByPage]
    @PageNo         INT,
    @ItemPerPage    INT,
    @OrderBy        INT,
    @LookFor        NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNo - 1) * @ItemPerPage;

    WITH CategoryCounts AS (
        SELECT 
            c.Id, 
            c.CategoryName, 
            COUNT(s.Id) AS SubcategoryCount, 
            COUNT(p.Id) AS ProductCount
        FROM 
            [dbo].[Categories] c
            LEFT JOIN [dbo].[SubCategories] s ON s.CategoryId = c.Id
            LEFT JOIN [dbo].[Products] p ON p.CategoryID = c.Id
        WHERE 
            c.MarkAsDeleted != 1 AND
            (@LookFor IS NULL OR c.CategoryName LIKE '%' + @LookFor + '%')
        GROUP BY 
            c.Id, 
            c.CategoryName
    )
    SELECT 
        Id, 
        CategoryName, 
        SubcategoryCount, 
        ProductCount
    FROM 
        CategoryCounts
    ORDER BY 
        CASE WHEN @OrderBy = 0 THEN CategoryName END ASC, 
        CASE WHEN @OrderBy = 1 THEN CategoryName END DESC,
        CASE WHEN @OrderBy = 2 THEN CAST(ProductCount AS INT) END ASC,
        CASE WHEN @OrderBy = 3 THEN CAST(ProductCount AS INT) END DESC, 
        CASE WHEN @OrderBy = 4 THEN CAST(SubcategoryCount AS INT) END ASC,
        CASE WHEN @OrderBy = 5 THEN CAST(SubcategoryCount AS INT) END DESC 
    OFFSET @Offset ROWS
    FETCH NEXT @ItemPerPage ROWS ONLY

END