CREATE PROCEDURE [dbo].[spActivityLogInsert]
	@Id					INT, 
    @Message			NVARCHAR(MAX),
	@CreatedByUserId	INT,
    @Username			NVARCHAR(256),
    @Device				NVARCHAR(256),
    @Location			NVARCHAR(256),
    @StatusCode			INT,
	@CreatedAt			DATETIME2
AS

BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.ActivityLog(
		[Message],			
		[CreatedByUserId],	
		[Username],			
		[Device],			
		[Location],			
		[StatusCode],		
		[CreatedAt]) 
		
	VALUES(
		@Message,		
		@CreatedByUserId,
		@Username,		
		@Device,			
		@Location,		
		@StatusCode,		
		@CreatedAt)		

	SELECT @Id =  SCOPE_IDENTITY()
END

