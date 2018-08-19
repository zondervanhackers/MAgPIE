CREATE TABLE [Digital].[Resources]
(
	[ResourceId] INT NOT NULL PRIMARY KEY,
    [ResourceName] NVARCHAR(MAX) NOT NULL, 
    [ResourceType] VARCHAR(16) NOT NULL,
	[ResourcePlatform] NVARCHAR(MAX) NULL,
    [CreationDate] DATETIME2 NOT NULL DEFAULT sysdatetime() 
)
