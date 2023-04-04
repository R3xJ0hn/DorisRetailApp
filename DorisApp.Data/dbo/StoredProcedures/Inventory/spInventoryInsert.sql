CREATE PROCEDURE [dbo].[spInventoryInsert]
	@Id					INT, 
    @ProductID			INT, 
    @PurchasePrice		MONEY, 
    @RetailPrice		MONEY, 
    @Quantity			INT, 
    @IsAvailable		BIT,
    @Location			NCHAR(256), 
    @ExpiryDate			DATETIME2, 
    @PurchaseDate		DATETIME2, 
    @CreatedByUserId	INT,
	@UpdatedByUserId	INT,
	@CreatedAt			DATETIME2, 
	@UpdatedAt			DATETIME2
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.Inventory(
		[ProductID],
		[PurchasePrice],
		[RetailPrice],
		[Quantity],
		[IsAvailable],
		[Location],
		[ExpiryDate],
		[PurchaseDate],
		[CreatedByUserId],
		[UpdatedByUserId],
		[CreatedAt],
		[UpdatedAt]) 
		
	VALUES(
		@ProductID,
		@PurchasePrice,
		@RetailPrice,
		@Quantity,		
		@IsAvailable,	
		@Location,		
		@ExpiryDate,		
		@PurchaseDate,	
		@CreatedByUserId,
		@UpdatedByUserId,
		@CreatedAt,		
		@UpdatedAt)

	SELECT @Id =  SCOPE_IDENTITY()
END




