CREATE PROCEDURE [dbo].[spUserGetByEmail]
	@Email nvarchar(128)
AS
begin	
	set nocount on;
	SELECT *
	FROM dbo.Users
	WHERE EmailAddress = @Email
end				