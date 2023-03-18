CREATE PROCEDURE [dbo].[spBrandInsert]
	@Id					INT,
    @BrandName			NVARCHAR(256),
    @ThumbnailName		NVARCHAR(256),
	@CreatedByUserId	INT, 
	@UpdatedByUserId	INT, 
	@CreatedAt			DATETIME2,
	@UpdatedAt			DATETIME2
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.Brands(
		[BrandName],
		[ThumbnailName],
		[CreatedByUserId],
		[UpdatedByUserId],
		[CreatedAt],
		[UpdatedAt]) 
		
	VALUES(
	    @BrandName,		
	    @ThumbnailName,	
		@CreatedByUserId,
		@UpdatedByUserId,
		@CreatedAt,		
		@UpdatedAt)

	SELECT @Id =  SCOPE_IDENTITY()
END