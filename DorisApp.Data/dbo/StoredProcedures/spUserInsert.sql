CREATE PROCEDURE [dbo].[spUserInsert]
	@Id					INT,
	@RoleID				INT,
	@FirstName			NVARCHAR(50), 
	@LastName			NVARCHAR(50),
	@EmailAddress		NVARCHAR(256), 
	@PasswordHash		NVARCHAR(MAX), 
	@LastPasswordHash	NVARCHAR(MAX), 
	@LastPasswordCanged DATETIME2,
	@Token				NVARCHAR(MAX),
    @TokenCreated		DATETIME2,
    @TokenExpires		DATETIME2, 
	@CreatedAt			DATETIME2,
	@UpdatedAt			DATETIME2

AS

BEGIN
	set nocount on;

	INSERT INTO dbo.Users(
			[RoleID],			
			[FirstName],			
			[LastName],			
			[EmailAddress],		
			[PasswordHash],	
			[LastPasswordHash],	
			[LastPasswordCanged],
			[Token],
			[TokenCreated],
			[TokenExpires],
			[CreatedAt],			
			[UpdatedAt]) 
	VALUES(			
			@RoleID,				
			@FirstName,			
			@LastName,			
			@EmailAddress,		
			@PasswordHash,		
			@LastPasswordHash,
			@LastPasswordCanged,
			@Token,			
			@TokenCreated,		
			@TokenExpires,	
			@CreatedAt,			
			@UpdatedAt)

	SELECT @Id =  SCOPE_IDENTITY()
END