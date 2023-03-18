CREATE PROCEDURE [dbo].[spSubCategoryGetById]
	@Id nvarchar(128)
AS
begin	
	set nocount on;
	SELECT *
	FROM dbo.SubCategories
	WHERE id = @id
end		