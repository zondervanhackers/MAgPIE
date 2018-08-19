CREATE TABLE [Digital].[ResourceIdentifiers]
(
	[ResourceId] INT NOT NULL , 
    [IdentifierType] VARCHAR(16) NOT NULL, 
    [IdentifierValue] NVARCHAR(64) NOT NULL, 
	[ResourceType] VARCHAR(16) NOT NULL, 
    [CreationDate] DATETIME2 NOT NULL DEFAULT sysdatetime(), 
    CONSTRAINT [FK_ResourceIdentifiers_Resources] FOREIGN KEY ([ResourceId]) REFERENCES [Digital].[Resources]([ResourceId]), 
    PRIMARY KEY ([ResourceType], [ResourceId], [IdentifierType], [IdentifierValue]) 
	
)

GO

CREATE UNIQUE INDEX [ResourceIndex] ON [Digital].[ResourceIdentifiers] (ResourceId, IdentifierType, IdentifierValue, ResourceType)
