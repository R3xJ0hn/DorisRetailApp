CREATE PROCEDURE [dbo].[spProductGetIdentical]
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
begin	

	SET NOCOUNT ON;
	   SELECT *
       FROM Products
       WHERE ((LOWER(CONCAT(ProductName, Size)) = LOWER(CONCAT(@ProductName, @Size)) 
       AND (BrandID = @BrandID Or BrandID = '0') ) OR sku = @Sku) AND Id != 1;
end		











