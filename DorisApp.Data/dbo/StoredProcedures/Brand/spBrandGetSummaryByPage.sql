CREATE PROCEDURE [dbo].[spBrandGetSummaryByPage]
    @PageNo INT,
    @ItemPerPage INT,
    @OrderBy INT,
    @LookFor NVARCHAR(256) = NULL
    AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNo - 1) * @ItemPerPage;

    WITH BrandCounts AS (
        SELECT 
            b.Id, 
            b.BrandName,
            b.ThumbnailName,
            COUNT(p.Id) AS ProductCount
        FROM 
            [dbo].[Brands] b
            LEFT JOIN [dbo].[Products] p ON p.BrandID = b.Id
        WHERE 
            b.MarkAsDeleted != 1 AND
            (@LookFor IS NULL OR b.BrandName LIKE '%' + @LookFor + '%')
        GROUP BY 
            b.Id, 
            b.BrandName,
            b.ThumbnailName)
        SELECT 
            Id, 
            BrandName,
            ThumbnailName,
            ProductCount
        FROM 
            BrandCounts
        ORDER BY 
            CASE WHEN @OrderBy = 0 THEN BrandName END ASC, 
            CASE WHEN @OrderBy = 1 THEN BrandName END DESC,
            CASE WHEN @OrderBy = 2 THEN CAST(ProductCount AS INT) END ASC,
            CASE WHEN @OrderBy = 3 THEN CAST(ProductCount AS INT) END DESC

        OFFSET @Offset ROWS
        FETCH NEXT @ItemPerPage ROWS ONLY
END