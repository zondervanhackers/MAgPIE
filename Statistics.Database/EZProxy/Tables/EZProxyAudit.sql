CREATE TABLE [EZProxy].[EZProxyAudit]
(
	[DateTime] DATETIME NOT NULL, 
    [Event] NVARCHAR(50) NOT NULL, 
    [IP] NVARCHAR(50) NULL, 
    [Username] NVARCHAR(100) NULL, 
    [Session] NVARCHAR(50) NULL, 
    [Other] NVARCHAR(300) NULL, 
    [LineNumber] INT NOT NULL, 
    [CreatedDate] DATETIME2 NOT NULL, 
    [ModifiedDate] DATETIME2 NOT NULL, 
    CONSTRAINT [PK_EZProxyAudit] PRIMARY KEY ([DateTime], [Event], [LineNumber])
)
