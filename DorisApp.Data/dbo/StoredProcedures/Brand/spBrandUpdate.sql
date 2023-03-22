CREATE PROCEDURE [dbo].[spBrandUpdate]
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

		UPDATE dbo.Brands SET
			[BrandName] = @BrandName,
			[StoredImageName] = @StoredImageName,
			[UpdatedByUserId] = @UpdatedByUserId,
			[UpdatedAt] = @UpdatedAt

	WHERE [Id] = @Id

END