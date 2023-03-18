CREATE PROCEDURE [dbo].[spUserGetById]
	@Id INT
AS
begin	
	set nocount on;
	SELECT *
	FROM dbo.Users
	WHERE id = @id
end				