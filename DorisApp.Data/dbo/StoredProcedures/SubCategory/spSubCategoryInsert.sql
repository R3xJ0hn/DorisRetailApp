CREATE PROCEDURE [dbo].[spSubCategoryInsert]
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

	INSERT INTO dbo.SubCategories(
		[SubCategoryName],	
		[CategoryId],
		[CreatedByUserId],	
		[UpdatedByUserId],	
		[CreatedAt],		
		[UpdatedAt]) 
	VALUES(
		@SubCategoryName,
		@CategoryId,
		@CreatedByUserId,		
		@UpdatedByUserId,		
		@CreatedAt,				
		@UpdatedAt)

	SELECT @Id =  SCOPE_IDENTITY()
END