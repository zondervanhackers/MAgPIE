CREATE TABLE [Digital].[ResourceRecords]
(
	[ResourceRecordId] BIGINT NOT NULL PRIMARY KEY, 
	[ParentResourceRecordId] BIGINT NULL, 
    [ResourceId] INT NOT NULL,
    [RunDate] DATE NOT NULL, 
    [CreationDate] DATETIME2 NOT NULL DEFAULT sysdatetime(), 
    [ModifiedDate] DATETIME2 NOT NULL DEFAULT sysdatetime(), 
    CONSTRAINT [FK_ResourceRecords_Resources] FOREIGN KEY ([ResourceId]) REFERENCES [Digital].[Resources]([ResourceId])
)