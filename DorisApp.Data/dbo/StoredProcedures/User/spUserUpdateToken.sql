CREATE PROCEDURE [dbo].[spUserUpdateToken]
	@Id					INT,
	@RoleID				INT,
	@FirstName			NVARCHAR(256), 
	@LastName			NVARCHAR(256),
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

	UPDATE dbo.Users SET
			[Token] = @Token,
			[TokenCreated] = @TokenCreated,
			[TokenExpires] = @TokenExpires

	WHERE [Id] = @Id

END