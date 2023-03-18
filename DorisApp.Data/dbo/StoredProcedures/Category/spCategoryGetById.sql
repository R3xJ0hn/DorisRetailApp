CREATE PROCEDURE [dbo].[spCategoryGetById]
	@Id nvarchar(256)
AS
begin	
	set nocount on;
	SELECT *
	FROM dbo.Categories
	WHERE id = @id
end		