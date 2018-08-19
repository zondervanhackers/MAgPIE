CREATE TABLE [Persistance].[DirectoryRecords]
(
	[OperationID] INT NOT NULL,
    [RepositoryID] INT NOT NULL, 
    [FilePath] NVARCHAR(1024) NOT NULL, 
    [FileModifiedDate] DATETIME2 NOT NULL, 
	[CreationDate] DATETIME2 NOT NULL DEFAULT (sysdatetime()),
    [ModifiedDate] DATETIME2 NOT NULL DEFAULT (sysdatetime()), 
    CONSTRAINT [PK_DirectoryRecords] PRIMARY KEY ([RepositoryID], [OperationID], [FilePath]),
    CONSTRAINT [FK_DirectoryRecords_Operations] FOREIGN KEY ([OperationID]) REFERENCES [Persistance].[Operations]([ID]),
    CONSTRAINT [FK_DirectoryRecords_Repositories] FOREIGN KEY ([RepositoryID]) REFERENCES [Persistance].[Repositories]([ID])
)
