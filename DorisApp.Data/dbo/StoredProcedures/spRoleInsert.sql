CREATE PROCEDURE [dbo].[spRoleInsert]
	@Id					INT,
    @RoleName			NVARCHAR(100),
	@CreatedByUserId	INT,
	@UpdatedByUserId	INT,
	@CreatedAt			DATETIME2,
	@UpdatedAt			DATETIME2
AS

BEGIN
	set nocount on;

	INSERT INTO dbo.Roles(
		[RoleName],	
		[CreatedByUserId],	
		[UpdatedByUserId],	
		[CreatedAt],		
		[UpdatedAt]) 
	VALUES(
		@RoleName,				
		@CreatedByUserId,		
		@UpdatedByUserId,		
		@CreatedAt,				
		@UpdatedAt)

	SELECT @Id =  SCOPE_IDENTITY()
END