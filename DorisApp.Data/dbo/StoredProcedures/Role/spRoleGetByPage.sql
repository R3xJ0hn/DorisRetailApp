CREATE PROCEDURE [dbo].[spRoleGetByPage]
	@PageNo	INT
AS

BEGIN
	set nocount on;

	SELECT * 
	FROM Roles
	ORDER BY [RoleName] ASC
	OFFSET (@PageNo - 1) * 50 ROWS
	FETCH NEXT 50 ROWS ONLY

END
