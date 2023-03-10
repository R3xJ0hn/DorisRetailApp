CREATE PROCEDURE [dbo].[spUserGetById]
	@Id nvarchar(128)
AS
begin	
	set nocount on;
	SELECT *
	FROM dbo.Users
	WHERE id = @id
end				