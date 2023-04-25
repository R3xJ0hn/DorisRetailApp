CREATE TABLE [dbo].[Inventory]
(
	[Id]                INT NOT NULL PRIMARY KEY IDENTITY, 
    [ProductId]         INT NOT NULL, 
    [Sku]               NCHAR(256) NOT NULL, 
    [PurchasePrice]     MONEY NOT NULL, 
    [RetailPrice]       MONEY NOT NULL, 
    [Quantity]          INT NOT NULL, 
    [StockRemain]       INT NOT NULL, 
    [StockAvailable]    INT NOT NULL, 
    [IsAvailable]		BIT NOT NULL DEFAULT 1,
    [Location]          NCHAR(100) NOT NULL, 
    [ExpiryDate]        DATETIME2 NULL, 
    [PurchasedDate]     DATETIME2 NOT NULL DEFAULT getutcdate(), 
    [ReasonPhrase]      NVARCHAR(MAX) NULL, 
    [CreatedByUserId]	INT DEFAULT 0 NOT NULL,
	[UpdatedByUserId]	INT DEFAULT 0 NOT NULL,
	[CreatedAt]			DATETIME2 NOT NULL DEFAULT getutcdate(), 
	[UpdatedAt]			DATETIME2 NOT NULL DEFAULT getutcdate()

    CONSTRAINT [FK_Inventory_ToProducts] FOREIGN KEY ([ProductID]) REFERENCES [Products]([Id]), 
)
