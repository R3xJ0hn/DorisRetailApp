CREATE PROCEDURE [dbo].[spCategoryInsert]
	@Id					INT,
    @CategoryName			NVARCHAR(100),
	@CreatedByUserId	INT,
	@UpdatedByUserId	INT,
	@CreatedAt			DATETIME2,
	@UpdatedAt			DATETIME2
AS

BEGIN
	set nocount on;

	INSERT INTO dbo.Categories(
		[CategoryName],	
		[CreatedByUserId],	
		[UpdatedByUserId],	
		[CreatedAt],		
		[UpdatedAt]) 
	VALUES(
		@CategoryName,				
		@CreatedByUserId,		
		@UpdatedByUserId,		
		@CreatedAt,				
		@UpdatedAt)

	SELECT @Id =  SCOPE_IDENTITY()
END