CREATE TABLE [Patron].[StudentResidences]
(
	[StudentResidenceId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[ResidenceName] VARCHAR(32) NOT NULL,
	[ResidenceCategory] VARCHAR(16) NOT NULL,
	[CreationDate] DATETIME2(7) NOT NULL DEFAULT (sysdatetime()),
	[ModifiedDate] DATETIME2(7) NOT NULL DEFAULT (sysdatetime())
)

GO

CREATE UNIQUE INDEX [IX_StudentResidences_ResidenceName] ON [Patron].[StudentResidences] ([ResidenceName])