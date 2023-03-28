CREATE PROCEDURE [dbo].[spProductRestore]
	@Id					INT,
	@ProductName		NVARCHAR(256),
	@BrandID			INT,  
	@CategoryID         INT, 
	@SubcategoryID      INT, 
	@IsTaxable			BIT,
	@IsAvailable        BIT,
	@Size				NVARCHAR(256),
	@Color				NVARCHAR(256),
	@Sku				NVARCHAR(256), 
	@Description		NVARCHAR(MAX), 
	@StoredImageName	NVARCHAR(256),
	@CreatedByUserId	INT,
	@UpdatedByUserId	INT,
	@CreatedAt			DATETIME2,
	@UpdatedAt			DATETIME2
AS

BEGIN
	SET NOCOUNT ON;

		UPDATE dbo.Products SET
			[StoredImageName] = @StoredImageName,
			[MarkAsDeleted] = 0

	WHERE [Id] = @Id

	SELECT @Id =  SCOPE_IDENTITY()
END  