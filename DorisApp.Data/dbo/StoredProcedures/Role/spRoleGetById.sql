CREATE PROCEDURE [dbo].[spRoleGetById]
	@Id nvarchar(128)
AS
begin	
	set nocount on;
	SELECT *
	FROM dbo.Roles
	WHERE id = @id
end				