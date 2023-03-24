CREATE TABLE [dbo].[Products]
(
	[Id]			    INT NOT NULL PRIMARY KEY IDENTITY, 
    [ProductName]       NVARCHAR(256) NOT NULL,
    [BrandID]           INT NOT NULL, 
    [CategoryID]        INT NOT NULL, 
    [SubcategoryID]     INT NOT NULL, 
    [IsTaxable]         BIT NULL DEFAULT 0,
    [IsAvailable]       BIT NULL DEFAULT 0,
    [Size]              NVARCHAR(256) NULL, 
    [Color]             NVARCHAR(256) NULL, 
    [Sku]               NCHAR(256) NOT NULL, 
    [Description]       NVARCHAR(MAX) NULL, 
    [StoredImageName]   NVARCHAR(MAX) NULL, 
	[CreatedByUserId]	INT DEFAULT 0 NOT NULL,
	[UpdatedByUserId]	INT DEFAULT 0 NOT NULL,
	[CreatedAt]			DATETIME2 NOT NULL DEFAULT getutcdate(), 
	[UpdatedAt]			DATETIME2 NOT NULL DEFAULT getutcdate(), 
    [MarkAsDeleted]     BIT NOT NULL DEFAULT 0, 

    CONSTRAINT [FK_Products_ToCategory] FOREIGN KEY ([CategoryID]) REFERENCES [Categories]([Id]),
    CONSTRAINT [FK_Products_ToSubCategory] FOREIGN KEY ([SubCategoryID]) REFERENCES [SubCategories]([Id]) 
)