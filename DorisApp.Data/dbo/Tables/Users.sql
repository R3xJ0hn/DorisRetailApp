CREATE TABLE [dbo].[Users]
(
	[Id]                 INT NOT NULL PRIMARY KEY IDENTITY, 
	[RoleID]			 INT NOT NULL,
	[FirstName]			 NVARCHAR(50) NOT NULL, 
	[LastName]			 NVARCHAR(50) NOT NULL,
	[EmailAddress]		 NVARCHAR(256) NOT NULL UNIQUE, 
	[PasswordHash]		 NVARCHAR(MAX) NOT NULL, 
	[LastPasswordHash]	 NVARCHAR(MAX) NOT NULL, 
	[LastPasswordCanged] DATETIME2 NOT NULL,
	[Token]				 NVARCHAR(MAX) NOT NULL,
    [TokenCreated]		 DATETIME2 NOT NULL, 
    [TokenExpires]		 DATETIME2 NOT NULL, 
	[CreatedAt]			 DATETIME2 NOT NULL DEFAULT getutcdate(), 
	[UpdatedAt]			 DATETIME2 NOT NULL DEFAULT getutcdate(), 
    [MarkAsDeleted] BIT NOT NULL DEFAULT 0, 
)
