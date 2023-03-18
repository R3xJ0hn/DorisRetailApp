CREATE PROCEDURE [dbo].[spCategoryUpdate]
	@Id					INT,
    @CategoryName		NVARCHAR(256),
	@CreatedByUserId	INT,
	@UpdatedByUserId	INT,
	@CreatedAt			DATETIME2,
	@UpdatedAt			DATETIME2
AS

BEGIN
	set nocount on;

		UPDATE dbo.Categories SET
			[CategoryName] = @CategoryName,
			[UpdatedByUserId] = @UpdatedByUserId,
			[UpdatedAt] = @UpdatedAt

	WHERE [Id] = @Id

END