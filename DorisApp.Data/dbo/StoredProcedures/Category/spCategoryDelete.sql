CREATE PROCEDURE [dbo].[spCategoryDelete]
	@Id					INT,
    @CategoryName		NVARCHAR(100),
	@CreatedByUserId	INT,
	@UpdatedByUserId	INT,
	@CreatedAt			DATETIME2,
	@UpdatedAt			DATETIME2
AS

BEGIN
	set nocount on;

		UPDATE dbo.Categories SET
			[MarkAsDeleted] = 1

	WHERE [Id] = @Id

END