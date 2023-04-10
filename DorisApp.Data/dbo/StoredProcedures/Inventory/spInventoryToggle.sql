CREATE PROCEDURE [dbo].[spInventoryToggle]
	@Id				INT,
	@IsAvailable	BIT
AS

BEGIN
	SET NOCOUNT ON;

	UPDATE dbo.Inventory SET
	[IsAvailable] = @IsAvailable 
	WHERE [Id] = @Id

END