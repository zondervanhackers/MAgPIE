CREATE TABLE [Patron].[StudentMajorRecords]
(
	[StudentRecordId] INT NOT NULL,
	[MajorType] varchar(8) NOT NULL,
	[StudentMajorId] INT NOT NULL, 
	[CreationDate] DATETIME2(7) NOT NULL DEFAULT (sysdatetime()),
	[ModifiedDate] DATETIME2(7) NOT NULL DEFAULT (sysdatetime()),
	PRIMARY KEY ([StudentRecordId], [MajorType]),
    CONSTRAINT [FK_StudentMajorRecords_StudentRecords] FOREIGN KEY ([StudentRecordId]) REFERENCES [Patron].[StudentRecords]([StudentRecordId]) ON DELETE CASCADE, 
    CONSTRAINT [FK_StudentMajorRecords_StudentMajors] FOREIGN KEY ([StudentMajorId]) REFERENCES [Patron].[StudentMajors]([StudentMajorId])
)

GO

CREATE INDEX [IX_StudentMajorRecords_StudentMajorId] ON [Patron].[StudentMajorRecords] (StudentMajorId)