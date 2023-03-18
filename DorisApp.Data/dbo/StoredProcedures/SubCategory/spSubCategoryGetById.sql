CREATE PROCEDURE [dbo].[spSubCategoryGetById]
	@Id INT
AS
begin	
	set nocount on;
	SELECT *
	FROM dbo.SubCategories
	WHERE id = @id
end		