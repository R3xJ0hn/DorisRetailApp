CREATE PROCEDURE [dbo].[spSubCategoryGetIdentical]
	@Id					INT,
    @SubCategoryName	NVARCHAR(256),
    @CategoryId			INT,
	@CreatedByUserId	INT,
	@UpdatedByUserId	INT,
	@CreatedAt			DATETIME2,
	@UpdatedAt			DATETIME2
AS
	
	SELECT *
	FROM dbo.SubCategories
	WHERE SubCategoryName = @SubCategoryName AND Id != 1

RETURN 0
