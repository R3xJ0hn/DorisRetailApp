CREATE PROCEDURE [dbo].[spCategoryGetByPage]
	@PageNo				INT,
	@ItemPerPage		INT,
	@OrderBy			INT
AS

BEGIN
	set nocount on;

	DECLARE @Offset INT = (@PageNo - 1) * @ItemPerPage;

	SELECT *
	FROM [Categories]
	ORDER BY
		CASE 
			WHEN @OrderBy = 1 THEN [CategoryName]
			ELSE NULL
		END ASC,
		CASE 
			WHEN @OrderBy = 2 THEN [CategoryName]
			ELSE NULL
		END DESC
	OFFSET @Offset ROWS
	FETCH NEXT @ItemPerPage ROWS ONLY;

END
