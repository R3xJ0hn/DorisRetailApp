CREATE PROCEDURE [dbo].[spBrandGetById] 
	@Id INT
AS
begin	
	SET NOCOUNT ON;

	SELECT *
	FROM dbo.Categories
	WHERE id = @id
end		