CREATE PROCEDURE [dbo].[spSalesGetAvailableProducts]
    @PageNo         INT,
    @ItemPerPage    INT,
    @OrderBy        INT,
    @LookFor        NVARCHAR(256) = NULL,
    @Sku            NVARCHAR(50) = NULL,
    @CategoryId     INT = -1,
    @SubCategoryId  INT = -1
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNo - 1) * @ItemPerPage;

    IF @Sku IS NULL AND  @CategoryId < 0 AND  @SubCategoryId < 0
    BEGIN
        SELECT 
            i.Id,
            p.Id AS ProductId,
            p.ProductName,
            p.Sku,
            p.Color,
            p.Size,
            i.RetailPrice,
            i.StockAvailable,
            p.StoredImageName,
            p.IsTaxable
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
    ELSE
    BEGIN

        IF @Sku IS NULL 
        BEGIN
            IF (@SubCategoryId >= 0)
            BEGIN
               SET @CategoryId = -1
            END

            IF @CategoryId > 0 AND  @SubCategoryId < 0
            BEGIN

                SELECT 
                    i.Id,
                    p.Id AS ProductId,
                    p.ProductName,
                    p.Sku,
                    p.Color,
                    p.Size,
                    i.RetailPrice,
                    i.StockAvailable,
                    p.StoredImageName,
                    p.IsTaxable
                FROM 
                    [dbo].[Products] p
                    INNER JOIN [dbo].[Inventory] i ON p.Id = i.ProductId
                WHERE 
                    p.IsAvailable = 1 AND i.IsAvailable = 1 AND i.StockAvailable > 0 AND p.CategoryID = @CategoryId
                ORDER BY 
                    CASE WHEN @OrderBy = 0 THEN ProductName END ASC, 
                    CASE WHEN @OrderBy = 1 THEN ProductName END DESC

                OFFSET @Offset ROWS
                FETCH NEXT @ItemPerPage ROWS ONLY

            END

            
            IF @SubCategoryId > 0
            BEGIN
                
               SELECT 
                    i.Id,
                    p.Id AS ProductId,
                    p.ProductName,
                    p.Sku,
                    p.Color,
                    p.Size,
                    i.RetailPrice,
                    i.StockAvailable,
                    p.StoredImageName,
                    p.IsTaxable
                FROM 
                    [dbo].[Products] p
                    INNER JOIN [dbo].[Inventory] i ON p.Id = i.ProductId
                WHERE 
                    p.IsAvailable = 1 AND i.IsAvailable = 1 AND i.StockAvailable > 0 AND p.SubcategoryID = @SubCategoryId
                ORDER BY 
                    CASE WHEN @OrderBy = 0 THEN ProductName END ASC, 
                    CASE WHEN @OrderBy = 1 THEN ProductName END DESC

                OFFSET @Offset ROWS
                FETCH NEXT @ItemPerPage ROWS ONLY

            END

        END

        ELSE
        BEGIN

            SELECT 
                i.Id,
                p.Id AS ProductId,
                p.ProductName,
                p.Sku,
                p.Color,
                p.Size,
                i.RetailPrice,
                i.StockAvailable,
                p.StoredImageName,
                p.IsTaxable
            FROM 
                [dbo].[Products] p
                INNER JOIN [dbo].[Inventory] i ON p.Id = i.ProductId
            WHERE 
                p.IsAvailable = 1 AND i.IsAvailable = 1 AND i.StockAvailable > 0 AND p.Sku = @Sku
            ORDER BY 
                CASE WHEN @OrderBy = 0 THEN ProductName END ASC, 
                CASE WHEN @OrderBy = 1 THEN ProductName END DESC

            OFFSET @Offset ROWS
            FETCH NEXT @ItemPerPage ROWS ONLY

        END
    END

END
