CREATE PROCEDURE [dbo].[spProductGetSummaryByPage]
    @PageNo         INT,
    @ItemPerPage    INT,
    @OrderBy        INT,
    @LookFor        NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNo - 1) * @ItemPerPage;

    WITH ProductCounts AS (
    SELECT 
        p.Id, 
        p.ProductName,
        p.Sku, 
        p.StoredImageName, 
        p.IsAvailable,
        CASE 
            WHEN b.MarkAsDeleted = 1 THEN CONCAT(b.BrandName, ' *') 
            ELSE b.BrandName 
        END AS BrandName,
        COUNT(i.Id) AS InventoryCount,
        SUM(i.Quantity) AS TotalStock 
    FROM 
        [dbo].[Products] p
        LEFT JOIN [dbo].[Brands] b ON b.Id = p.BrandID
        LEFT JOIN [dbo].[Inventory] i ON i.ProductID = p.Id
    WHERE 
        p.MarkAsDeleted != 1 AND 
        (@LookFor IS NULL OR p.ProductName LIKE '%' + @LookFor + '%' OR p.Sku LIKE '%' + @LookFor + '%')
    GROUP BY 
        p.Id, 
        p.ProductName,
        p.Sku, 
        p.StoredImageName,
        p.IsAvailable,
        CASE 
            WHEN b.MarkAsDeleted = 1 THEN CONCAT(b.BrandName, ' *') 
            ELSE b.BrandName 
        END
    )
    SELECT 
        Id, 
        ProductName,
        Sku,
        BrandName,
        StoredImageName, 
        IsAvailable,
        InventoryCount,
        TotalStock
    FROM 
        ProductCounts
    ORDER BY 
        CASE WHEN @OrderBy = 0 THEN ProductName END ASC,
        CASE WHEN @OrderBy = 1 THEN ProductName END DESC
    OFFSET @Offset ROWS
    FETCH NEXT @ItemPerPage ROWS ONLY

END