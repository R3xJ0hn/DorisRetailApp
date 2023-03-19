CREATE PROCEDURE [dbo].[spBrandGetById] 
	@Id INT
AS
begin	
	SET NOCOUNT ON;

	SELECT *
	FROM dbo.Brands
	WHERE id = @id
end		