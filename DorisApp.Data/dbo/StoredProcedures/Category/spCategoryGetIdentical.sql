CREATE PROCEDURE [dbo].[spCategoryGetIdentical]
	@Id					INT,
    @CategoryName		NVARCHAR(256),
	@CreatedByUserId	INT,
	@UpdatedByUserId	INT,
	@CreatedAt			DATETIME2,
	@UpdatedAt			DATETIME2
AS
begin	
	SET NOCOUNT ON;

	SELECT *
	FROM dbo.Categories
	WHERE CategoryName = @CategoryName AND Id != 1
end		