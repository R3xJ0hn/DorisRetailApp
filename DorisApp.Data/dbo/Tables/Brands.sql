CREATE TABLE [dbo].[Brands]
(
	[Id]			    INT NOT NULL PRIMARY KEY IDENTITY, 
    [BrandName]			NVARCHAR(100) NOT NULL,
    [ThumbnailName]		NVARCHAR(100) NULL,
	[CreatedByUserId]	INT DEFAULT 0 NOT NULL,
	[UpdatedByUserId]	INT DEFAULT 0 NOT NULL,
	[CreatedAt]			DATETIME2 NOT NULL DEFAULT getutcdate(), 
	[UpdatedAt]			DATETIME2 NOT NULL DEFAULT getutcdate(), 
    [MarkAsDeleted]		BIT NOT NULL DEFAULT 0
)
