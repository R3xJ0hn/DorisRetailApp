CREATE PROCEDURE [dbo].[spSubCategoryDelete]
	@Id					INT,
    @SubCategoryName	NVARCHAR(100),
    @CategoryId			INT,
	@CreatedByUserId	INT,
	@UpdatedByUserId	INT,
	@CreatedAt			DATETIME2,
	@UpdatedAt			DATETIME2
AS

BEGIN
	set nocount on;

		UPDATE dbo.SubCategories SET
			[MarkAsDeleted] = 1

	WHERE [Id] = @Id

END