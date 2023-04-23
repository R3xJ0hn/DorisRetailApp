CREATE PROCEDURE [dbo].[spSalesGetAvailableProducts]
    @PageNo         INT,
    @ItemPerPage    INT,
    @OrderBy        INT,
    @LookFor        NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNo - 1) * @ItemPerPage;

    SELECT 
        i.Id,
        p.Id AS ProductId,
        p.ProductName,
        p.Sku,
        p.Color,
        p.Size,
        i.RetailPrice,
        i.StockAvailable,
        p.StoredImageName
    FROM 
        [dbo].[Products] p
        INNER JOIN [dbo].[Inventory] i ON p.Id = i.ProductId
    WHERE 
        p.IsAvailable = 1 AND i.IsAvailable = 1 AND i.StockAvailable > 0
    ORDER BY 
        CASE WHEN @OrderBy = 0 THEN ProductName END ASC, 
        CASE WHEN @OrderBy = 1 THEN ProductName END DESC

    OFFSET @Offset ROWS
    FETCH NEXT @ItemPerPage ROWS ONLY
END
