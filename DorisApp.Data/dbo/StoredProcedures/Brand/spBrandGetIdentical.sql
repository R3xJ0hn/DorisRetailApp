CREATE PROCEDURE [dbo].[spBrandGetIdentical]
	@Id					INT,
    @BrandName			NVARCHAR(256),
	@StoredImageName	NVARCHAR(256),
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
		END AS BrandName
	FROM dbo.Brands
	WHERE LOWER(BrandName) = LOWER(@BrandName) 

end		