CREATE PROCEDURE [dbo].[spCategoryGetByPage]
	@PageNo				INT,
	@ItemPerPage		INT,
	@OrderBy			NVARCHAR(120)
AS

BEGIN
	set nocount on;

	DECLARE @Offset INT = (@PageNo - 1) * @ItemPerPage;

	SELECT *
	FROM [Categories]
	ORDER BY
		CASE 
			WHEN @OrderBy = 'name-asc' THEN [CategoryName]
			ELSE NULL
		END ASC,
		CASE 
			WHEN @OrderBy = 'name-desc' THEN [CategoryName]
			ELSE NULL
		END DESC
	OFFSET @Offset ROWS
	FETCH NEXT @ItemPerPage ROWS ONLY;

END
