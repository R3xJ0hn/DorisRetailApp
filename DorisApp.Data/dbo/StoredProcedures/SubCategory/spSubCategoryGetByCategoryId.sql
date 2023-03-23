CREATE PROCEDURE [dbo].[spSubCategoryGetByCategoryId] 
	@CategoryId INT
AS
begin	
	set nocount on;
	SELECT *
	FROM dbo.SubCategories
	WHERE CategoryId = @CategoryId AND MarkAsDeleted = 0
end		