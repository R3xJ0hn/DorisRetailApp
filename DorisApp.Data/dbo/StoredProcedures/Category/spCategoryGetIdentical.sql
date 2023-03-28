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

	SELECT *,
		--The Dappper will overide the name.
		CASE 
			WHEN MarkAsDeleted = 1 THEN '*' 
			ELSE '' 
		END AS CategoryName
	FROM dbo.Categories
	WHERE LOWER(CategoryName) = LOWER(@CategoryName) 

end		