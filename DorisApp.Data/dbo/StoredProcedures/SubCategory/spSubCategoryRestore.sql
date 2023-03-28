CREATE PROCEDURE [dbo].[spSubCategoryRestore]
	@Id					INT,
    @SubCategoryName	NVARCHAR(256),
    @CategoryId			INT,
	@CreatedByUserId	INT,
	@UpdatedByUserId	INT,
	@CreatedAt			DATETIME2,
	@UpdatedAt			DATETIME2
AS

BEGIN
	set nocount on;

		UPDATE dbo.SubCategories SET
			[MarkAsDeleted] = 0

	WHERE [Id] = @Id

END