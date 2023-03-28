CREATE PROCEDURE [dbo].[spSubCategoryGetIdentical]
	@Id					INT,
    @SubCategoryName	NVARCHAR(256),
    @CategoryId			INT,
	@CreatedByUserId	INT,
	@UpdatedByUserId	INT,
	@CreatedAt			DATETIME2,
	@UpdatedAt			DATETIME2
AS
	
	SELECT *,
		--The Dappper will overide the name.
		CASE 
			WHEN MarkAsDeleted = 1 THEN '*' 
			ELSE '' 
		END AS SubCategoryName
	FROM dbo.SubCategories
	WHERE LOWER(SubCategoryName) = LOWER(@SubCategoryName) 

RETURN 0
