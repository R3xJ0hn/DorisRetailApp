CREATE PROCEDURE [dbo].[spCategoryGetTableDataByPage]
	@PageNo				INT,
	@ItemPerPage		INT,
	@OrderBy			INT
AS

BEGIN
	set nocount on;

	DECLARE @Offset INT = (@PageNo - 1) * @ItemPerPage;

    SELECT c.Id, c.CategoryName, COUNT(s.Id) AS SubcategoryCount, COUNT(p.Id) AS ProductCount
    FROM [dbo].[Categories] c
    LEFT JOIN  [dbo].[SubCategories] s ON c.Id = s.CategoryId
    LEFT JOIN  [dbo].[Products] p ON s.Id = p.SubcategoryID
    GROUP BY c.Id, c.CategoryName
	ORDER BY c.CategoryName ASC
    OFFSET @Offset ROWS
	FETCH NEXT @ItemPerPage ROWS ONLY

END
