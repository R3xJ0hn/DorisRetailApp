CREATE PROCEDURE [dbo].[spProductGetById]
	@Id INT
AS
begin	
	SET NOCOUNT ON;

	SELECT *
	FROM dbo.Products
	WHERE id = @id
end		
