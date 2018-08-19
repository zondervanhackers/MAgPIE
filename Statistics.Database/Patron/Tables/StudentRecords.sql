CREATE TABLE [Patron].[StudentRecords]
(
	[StudentRecordId] INT NOT NULL PRIMARY KEY,
	[PatronRecordId] INT NOT NULL,
	[StudentLevel] VARCHAR(16) NOT NULL, 
    [StudentType] VARCHAR(64) NOT NULL,
	[StudentClass] VARCHAR(16) NULL, 
    [CumulativeGpa] DECIMAL(3, 2) NULL, 
	[CreationDate] DATETIME2(7) NOT NULL DEFAULT (sysdatetime()),
	[ModifiedDate] DATETIME2(7) NOT NULL DEFAULT (sysdatetime()),
	CONSTRAINT [FK_StudentRecords_PatronRecords] FOREIGN KEY ([PatronRecordId]) REFERENCES [Patron].[PatronRecords]([PatronRecordId]) ON DELETE CASCADE
)

GO

CREATE INDEX [IX_StudentRecords_PatronRecordId] ON [Patron].[StudentRecords] ([PatronRecordId])