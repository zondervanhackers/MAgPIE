CREATE TABLE [Patron].[PatronRecords]
(
	[PatronRecordId] INT NOT NULL PRIMARY KEY,
	[PatronId] INT NOT NULL,
	[EffectiveStartDate] DATE NOT NULL, 
    [EffectiveEndDate] DATE NOT NULL, 
    [CreationDate] DATETIME2 NOT NULL DEFAULT (sysdatetime()), 
    [ModifiedDate] DATETIME2 NOT NULL DEFAULT (sysdatetime()), 
    CONSTRAINT [FK_PatronRecords_Patrons] FOREIGN KEY ([PatronId]) REFERENCES [Patron].[Patrons]([PatronId]), 
)

GO

CREATE UNIQUE INDEX [IX_PatronRecords_PatronId_EffectiveStartDate_EffectiveEndDate] ON [Patron].[PatronRecords] ([PatronId], [EffectiveStartDate], [EffectiveEndDate])