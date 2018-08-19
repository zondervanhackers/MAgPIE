CREATE TABLE [Circulation].[InventoryAnomalies]
(
	[OclcNumber] NVARCHAR(16) NULL, 
    [Title] NVARCHAR(MAX) NULL, 
    [MaterialFormat] NVARCHAR(32) NOT NULL, 
    [Author] NVARCHAR(MAX) NULL, 
    [Barcode] NVARCHAR(32) NULL, 
    [Cost] NVARCHAR(16) NULL, 
    [LastInventoriedDate] DATETIME2 NULL, 
    [DeletedDate] DATETIME2 NULL, 
    [ItemType] NVARCHAR(16) NOT NULL, 
    [CallNumber] NVARCHAR(64) NULL, 
    [ShelvingLocation] NVARCHAR(32) NULL, 
    [CurrentStatus] NVARCHAR(16) NULL, 
    [Description] NVARCHAR(MAX) NULL, 
    [RunDate] DATE NOT NULL, 
    [CountFound] INT NOT NULL 
)

GO

CREATE INDEX [IX_InventoryAnomalies_Column] ON [Circulation].[InventoryAnomalies] (Barcode, MaterialFormat, ItemType, RunDate)
