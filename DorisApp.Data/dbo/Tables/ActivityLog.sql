CREATE TABLE [dbo].[ActivityLog]
(
	[Id]				INT NOT NULL PRIMARY KEY IDENTITY, 
    [Message]			NVARCHAR(MAX) NOT NULL,
	[CreatedByUserId]	INT DEFAULT 0 NOT NULL,
    [Username]			NVARCHAR(256) NULL,
    [Device]			NVARCHAR(256) NULL,
    [Location]			NVARCHAR(256) NULL,
    [StatusCode]		INT DEFAULT 0 NOT NULL,
	[CreatedAt]			DATETIME2 NOT NULL DEFAULT getutcdate()
)


