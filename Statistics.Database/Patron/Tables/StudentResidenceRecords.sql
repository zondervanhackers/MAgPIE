CREATE TABLE [Patron].[StudentResidenceRecords]
(
	[StudentRecordId] INT NOT NULL PRIMARY KEY, 
    [StudentResidenceId] INT NOT NULL, 
	[CreationDate] DATETIME2(7) NOT NULL DEFAULT (sysdatetime()),
	[ModifiedDate] DATETIME2(7) NOT NULL DEFAULT (sysdatetime()),
    CONSTRAINT [FK_StudentResidenceRecords_StudentRecords] FOREIGN KEY ([StudentRecordId]) REFERENCES [Patron].[StudentRecords]([StudentRecordId]) ON DELETE CASCADE, 
    CONSTRAINT [FK_StudentResidenceRecords_StudentResidences] FOREIGN KEY ([StudentResidenceId]) REFERENCES [Patron].[StudentResidences]([StudentResidenceId])
)

GO

CREATE INDEX [IX_StudentResidenceRecords_StudentResidenceId] ON [Patron].[StudentResidenceRecords] (StudentResidenceId)