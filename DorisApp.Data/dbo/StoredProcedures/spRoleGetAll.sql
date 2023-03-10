CREATE PROCEDURE [dbo].[spRoleGetAll]
AS

BEGIN
	set nocount on;
		SELECT  *
		FROM dbo.Roles 
END