CREATE TABLE [dbo].[Roles]
(
	[Id]			    INT NOT NULL PRIMARY KEY IDENTITY, 
    [RoleName]			NVARCHAR(100) NOT NULL,
	[CreatedByUserId]	INT DEFAULT 0 NOT NULL,
	[UpdatedByUserId]	INT DEFAULT 0 NOT NULL,
	[CreatedAt]			DATETIME2 NOT NULL DEFAULT getutcdate(), 
	[UpdatedAt]			DATETIME2 NOT NULL DEFAULT getutcdate(), 
)
