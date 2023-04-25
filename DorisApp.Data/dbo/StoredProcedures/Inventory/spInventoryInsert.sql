CREATE PROCEDURE [dbo].[spInventoryInsert]
	@Id					INT, 
    @ProductID			INT, 
    @Sku				NVARCHAR(255),
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

	INSERT INTO dbo.Inventory(
		[ProductID],
		[SKU],
		[PurchasePrice],
		[RetailPrice],
		[Quantity],
		[StockRemain],
		[StockAvailable],
		[IsAvailable],
		[Location],
		[ExpiryDate],
		[PurchasedDate],
		[CreatedByUserId],
		[UpdatedByUserId],
		[CreatedAt],
		[UpdatedAt]) 
		
	VALUES(
		@ProductID,
		@Sku,
		@PurchasePrice,
		@RetailPrice,
		@Quantity,		
		@StockRemain,
		@StockAvailable,
		@IsAvailable,	
		@Location,		
		@ExpiryDate,		
		@PurchasedDate,	
		@CreatedByUserId,
		@UpdatedByUserId,
		@CreatedAt,		
		@UpdatedAt)

	SELECT @Id =  SCOPE_IDENTITY()
END