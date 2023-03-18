CREATE PROCEDURE [dbo].[spRoleGetById]
	@Id INT
AS
begin	
	set nocount on;
	SELECT *
	FROM dbo.Roles
	WHERE id = @id
end				