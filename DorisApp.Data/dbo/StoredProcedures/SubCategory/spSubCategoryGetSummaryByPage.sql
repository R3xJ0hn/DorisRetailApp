CREATE PROCEDURE [dbo].[spSubCategoryGetSummaryByPage]
    @PageNo         INT,
    @ItemPerPage    INT,
    @OrderBy        INT,
    @LookFor        NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNo - 1) * @ItemPerPage;

    WITH SubCategoryCounts AS (
    SELECT 
        s.Id, 
        s.CategoryId,
        s.SubCategoryName, 
        CASE 
            WHEN c.MarkAsDeleted = 1 THEN CONCAT(c.CategoryName, ' *') 
            ELSE c.CategoryName 
        END AS CategoryName,
        COUNT(p.Id) AS ProductCount
    FROM 
        [dbo].[SubCategories] s
        LEFT JOIN [dbo].[Categories] c ON c.Id = s.CategoryId
        LEFT JOIN [dbo].[Products] p ON p.SubCategoryID = s.Id
    WHERE 
        s.MarkAsDeleted != 1 AND
        (@LookFor IS NULL OR s.SubCategoryName LIKE '%' + @LookFor + '%')
    GROUP BY 
        s.Id, 
        s.CategoryId,
        s.SubCategoryName, 
        CASE 
            WHEN c.MarkAsDeleted = 1 THEN CONCAT(c.CategoryName, ' *') 
            ELSE c.CategoryName 
        END,
        c.CategoryName
    )
    SELECT 
        Id, 
        CategoryId,
        SubCategoryName,
        CategoryName, 
        ProductCount
    FROM 
        SubCategoryCounts
    ORDER BY 
        CASE WHEN @OrderBy = 0 THEN SubCategoryName END ASC,
        CASE WHEN @OrderBy = 1 THEN SubCategoryName END DESC,
        CASE WHEN @OrderBy = 2 THEN ProductCount END ASC, 
        CASE WHEN @OrderBy = 3 THEN ProductCount END DESC,
        CASE WHEN @OrderBy = 4 THEN CategoryName END ASC, 
        CASE WHEN @OrderBy = 5 THEN CategoryName END DESC
    OFFSET @Offset ROWS
    FETCH NEXT @ItemPerPage ROWS ONLY

END




