CREATE PROCEDURE [dbo].[spCategoryGetSummaryByPage]
    @PageNo         INT,
    @ItemPerPage    INT,
    @OrderBy        INT,
    @LookFor        NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNo - 1) * @ItemPerPage;

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
    ORDER BY 
        CASE 
            WHEN @OrderBy = 0 THEN c.CategoryName 
            ELSE '' 
        END ASC,
        CASE 
            WHEN @OrderBy = 1 THEN c.CategoryName 
            ELSE '' 
        END DESC
    OFFSET @Offset ROWS
    FETCH NEXT @ItemPerPage ROWS ONLY

END