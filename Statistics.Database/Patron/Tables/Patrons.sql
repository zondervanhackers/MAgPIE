CREATE TABLE [Patron].[Patrons]
(
	[PatronId] INT NOT NULL PRIMARY KEY,
	[Barcode] CHAR(14) NOT NULL,
	[Gender] VARCHAR(8) NOT NULL,
	[CreationDate] DATETIME2 NOT NULL DEFAULT(sysdatetime()),
	[ModifiedDate] DATETIME2 NOT NULL DEFAULT(sysdatetime())
)

GO

CREATE UNIQUE INDEX [IX_Patrons_Barcode] ON [Patron].[Patrons] ([Barcode])
