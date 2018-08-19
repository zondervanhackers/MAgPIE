CREATE TABLE [Persistance].[Repositories]
(
	[ID] INT NOT NULL IDENTITY, 
    [Name] NVARCHAR(200) NOT NULL, 
    CONSTRAINT [PK_Repositories] PRIMARY KEY ([ID]),
	CONSTRAINT [UK_Repositories] UNIQUE ([Name]), 
)
