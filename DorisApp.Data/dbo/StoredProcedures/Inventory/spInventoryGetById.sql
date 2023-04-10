CREATE PROCEDURE [dbo].[spInventoryGetById]
	@Id INT
AS
begin	
	SET NOCOUNT ON;

	SELECT *
	FROM dbo.Inventory
	WHERE id = @id
end		
