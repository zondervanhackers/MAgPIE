CREATE TABLE [Persistance].[OperationRecords]
(
	[OperationID] INT NOT NULL, 
    [RunDate] DATE NOT NULL, 
    [ExecutedDate] DATETIME NOT NULL DEFAULT (sysdatetime()) ,
	CONSTRAINT [PK_OperationRecords] PRIMARY KEY ([OperationID], [RunDate]), 
    CONSTRAINT [FK_OperationRecords_Operations] FOREIGN KEY ([OperationID]) REFERENCES [Persistance].[Operations]([ID])
)
