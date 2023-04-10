CREATE PROCEDURE [dbo].[spInventoryGetSummaryByPage]
    @PageNo         INT,
    @ItemPerPage    INT,
    @OrderBy        INT,
    @LookFor        NVARCHAR(256) = NULL,
    @IncludeSold    BIT,
    @StartDate      DATE = NULL,
    @EndDate        DATE = NULL,
    @Sku            NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNo - 1) * @ItemPerPage;

    IF @Sku IS NULL
    BEGIN
        SELECT 
            i.Id, 
            i.[Location],
            p.ProductName,
            p.Sku,
            p.IsAvailable as IsProductAvailable,
            i.StockRemain,
            i.PurchasedDate,
            i.ExpiryDate,
            i.IsAvailable
        FROM 
            [dbo].[Inventory] i
            LEFT JOIN [dbo].[Products] p ON i.ProductId = p.Id
        WHERE 
            (@LookFor IS NULL 
                OR i.[Location] LIKE '%' + @LookFor + '%' 
                OR p.ProductName LIKE '%' + @LookFor + '%' 
                OR p.Sku  LIKE @LookFor + '%' 
            )
            AND ((@StartDate IS NULL AND @EndDate IS NULL) 
                OR (i.PurchasedDate >= @StartDate AND i.PurchasedDate <= @EndDate)
            )
        ORDER BY 
            CASE WHEN @OrderBy = 0 THEN p.ProductName END ASC,
            CASE WHEN @OrderBy = 1 THEN p.ProductName END DESC,
            CASE WHEN @OrderBy = 2 THEN i.[Location] END ASC,
            CASE WHEN @OrderBy = 3 THEN i.[Location] END DESC,
            CASE WHEN @OrderBy = 4 THEN i.StockRemain END ASC, 
            CASE WHEN @OrderBy = 5 THEN i.StockRemain END DESC,
            CASE WHEN @OrderBy = 6 THEN i.PurchasedDate END ASC,
            CASE WHEN @OrderBy = 7 THEN i.PurchasedDate END DESC,
            CASE WHEN @OrderBy = 8 THEN i.ExpiryDate END ASC,
            CASE WHEN @OrderBy = 9 THEN i.ExpiryDate END DESC
    END
    ELSE

    BEGIN
        SELECT 
            i.Id, 
            i.[Location],
            p.ProductName,
            p.Sku,
            p.IsAvailable as IsProductAvailable,
            i.StockRemain,
            i.PurchasedDate,
            i.ExpiryDate,
            i.IsAvailable
        FROM 
            [dbo].[Inventory] i
            LEFT JOIN [dbo].[Products] p ON i.ProductId = p.Id
        WHERE 
            p.Sku = @Sku
        ORDER BY 
            CASE WHEN @OrderBy = 0 THEN p.ProductName END ASC,
            CASE WHEN @OrderBy = 1 THEN p.ProductName END DESC,
            CASE WHEN @OrderBy = 2 THEN i.[Location] END ASC,
            CASE WHEN @OrderBy = 3 THEN i.[Location] END DESC,
            CASE WHEN @OrderBy = 4 THEN i.StockRemain END ASC, 
            CASE WHEN @OrderBy = 5 THEN i.StockRemain END DESC,
            CASE WHEN @OrderBy = 6 THEN i.PurchasedDate END ASC,
            CASE WHEN @OrderBy = 7 THEN i.PurchasedDate END DESC,
            CASE WHEN @OrderBy = 8 THEN i.ExpiryDate END ASC,
            CASE WHEN @OrderBy = 9 THEN i.ExpiryDate END DESC
    END

END