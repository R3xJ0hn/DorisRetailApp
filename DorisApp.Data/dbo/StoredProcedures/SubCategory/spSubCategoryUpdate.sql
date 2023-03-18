CREATE PROCEDURE [dbo].[spSubCategoryUpdate]
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
			[SubCategoryName] = @SubCategoryName,
			[CategoryId] = @CategoryId,
			[UpdatedByUserId] = @UpdatedByUserId,
			[UpdatedAt] = @UpdatedAt

	WHERE [Id] = @Id

END