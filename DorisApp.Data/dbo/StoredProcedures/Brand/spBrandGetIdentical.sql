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

	SELECT *
	FROM dbo.Brands
	WHERE BrandName = @BrandName AND Id != 1
end		