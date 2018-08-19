CREATE TABLE [Persistance].[CounterOperationRecords]
(
	[OperationID] INT NOT NULL, 
	[RepositoryID] INT NOT NULL,
    [RunDate] DATE NOT NULL, 
	[Report] VARCHAR(5) NOT NULL, 
	[ExecutedDate] DATETIME NOT NULL DEFAULT (sysdatetime()) ,
    CONSTRAINT [PK_CounterOperationRecords] PRIMARY KEY ([OperationID], [RepositoryID], [RunDate], [Report]), 
    CONSTRAINT [FK_CounterOperationRecords_Operations] FOREIGN KEY ([OperationID]) REFERENCES [Persistance].[Operations]([ID]),
	CONSTRAINT [FK_CounterOperationRecords_Repositories] FOREIGN KEY ([RepositoryID]) REFERENCES [Persistance].[Repositories]([ID]),
)
