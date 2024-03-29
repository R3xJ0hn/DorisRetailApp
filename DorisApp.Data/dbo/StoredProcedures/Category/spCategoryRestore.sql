﻿CREATE PROCEDURE [dbo].[spCategoryRestore]
	@Id					INT,
    @CategoryName		NVARCHAR(256),
	@CreatedByUserId	INT,
	@UpdatedByUserId	INT,
	@CreatedAt			DATETIME2,
	@UpdatedAt			DATETIME2
AS

BEGIN
	set nocount on;

		UPDATE dbo.Categories SET
			[MarkAsDeleted] = 0

	WHERE [Id] = @Id

END