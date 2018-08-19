CREATE TABLE [Persistance].[Operations]
(
	[ID] INT NOT NULL IDENTITY, 
    [Name] NVARCHAR(200) NOT NULL,
    CONSTRAINT [PK_Operations] PRIMARY KEY ([ID]),
	CONSTRAINT [UK_Operations] Unique ([Name]), 
)
