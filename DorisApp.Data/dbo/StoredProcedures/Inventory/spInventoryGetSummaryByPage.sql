CREATE PROCEDURE [dbo].[spInventoryGetSummaryByPage]
    @PageNo         INT,
    @ItemPerPage    INT,
    @OrderBy        INT,
    @LookFor        NVARCHAR(256) = NULL,
    @IncludeSold    BIT,
    @StartDate      DATE = NULL,
    @EndDate        DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNo - 1) * @ItemPerPage;

    SELECT 
        i.Id, 
        i.[Location],
        p.ProductName,
        p.Sku,
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
        CASE WHEN @OrderBy = 0 THEN ProductName END ASC,
        CASE WHEN @OrderBy = 1 THEN ProductName END DESC,
        CASE WHEN @OrderBy = 2 THEN [Location] END ASC,
        CASE WHEN @OrderBy = 3 THEN [Location] END DESC,
        CASE WHEN @OrderBy = 4 THEN StockRemain END ASC, 
        CASE WHEN @OrderBy = 5 THEN StockRemain END DESC,
        CASE WHEN @OrderBy = 6 THEN PurchasedDate END ASC,
        CASE WHEN @OrderBy = 7 THEN PurchasedDate END DESC,
        CASE WHEN @OrderBy = 8 THEN ExpiryDate END ASC,
        CASE WHEN @OrderBy = 9 THEN ExpiryDate END DESC

    OFFSET @Offset ROWS
    FETCH NEXT @ItemPerPage ROWS ONLY

END
