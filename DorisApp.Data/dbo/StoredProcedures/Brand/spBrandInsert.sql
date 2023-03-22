CREATE PROCEDURE [dbo].[spBrandInsert]
	@Id					INT,
    @BrandName			NVARCHAR(256),
	@StoredImageName	NVARCHAR(256),
	@CreatedByUserId	INT, 
	@UpdatedByUserId	INT, 
	@CreatedAt			DATETIME2,
	@UpdatedAt			DATETIME2
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.Brands(
		[BrandName],
		[StoredImageName],
		[CreatedByUserId],
		[UpdatedByUserId],
		[CreatedAt],
		[UpdatedAt]) 
		
	VALUES(
	    @BrandName,		
		@StoredImageName,
		@CreatedByUserId,
		@UpdatedByUserId,
		@CreatedAt,		
		@UpdatedAt)

	SELECT @Id =  SCOPE_IDENTITY()
END