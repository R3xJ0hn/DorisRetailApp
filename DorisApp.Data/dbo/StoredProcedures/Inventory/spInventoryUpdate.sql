CREATE PROCEDURE [dbo].[spInventoryUpdate]
	@Id					INT, 
    @ProductID			INT, 
    @PurchasePrice		MONEY, 
    @RetailPrice		MONEY, 
    @Quantity			INT, 
	@StockRemain		INT,
	@StockAvailable		INT,
    @IsAvailable		BIT,
    @Location			NCHAR(256), 
    @ExpiryDate			DATETIME2, 
    @ReasonPhrase		NVARCHAR(MAX), 
    @PurchasedDate		DATETIME2, 
    @CreatedByUserId	INT,
	@UpdatedByUserId	INT,
	@CreatedAt			DATETIME2, 
	@UpdatedAt			DATETIME2,
	@SecurityStamp		NVARCHAR(MAX)
AS

BEGIN
	SET NOCOUNT ON;

	UPDATE dbo.Inventory SET
	[PurchasePrice] = @PurchasePrice,
	[RetailPrice] = @RetailPrice,
	[Quantity] = @Quantity,
	[Location] = @Location,
	[ExpiryDate] = @ExpiryDate,
	[PurchasedDate] = @PurchasedDate,
	[UpdatedByUserId] = @UpdatedByUserId,
	[UpdatedAt] = @UpdatedAt,
	[ReasonPhrase] = @ReasonPhrase,
	[StockRemain] = CASE 
						WHEN @Quantity > [Quantity] THEN [StockRemain] + (@Quantity - [Quantity]) -- If quantity added
						WHEN @Quantity < [Quantity] THEN [StockRemain] - ([Quantity] - @Quantity) -- If quantity subtracted
						ELSE [StockRemain] -- If quantity remains unchanged
					END,
	[StockAvailable] = CASE 
						WHEN @Quantity > [Quantity] THEN [StockAvailable] + (@Quantity - [Quantity]) -- If quantity added
						WHEN @Quantity < [Quantity] THEN [StockAvailable] - ([Quantity] - @Quantity) -- If quantity subtracted
						ELSE [StockAvailable] -- If quantity remains unchanged
					END,
	[IsAvailable] = @IsAvailable 
	WHERE [Id] = @Id

END