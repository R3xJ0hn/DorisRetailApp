CREATE PROCEDURE [dbo].[spBrandRestore]
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
			[StoredImageName] = @StoredImageName,
			[MarkAsDeleted] = 0

	WHERE [Id] = @Id

END