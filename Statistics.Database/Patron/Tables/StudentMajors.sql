CREATE TABLE [Patron].[StudentMajors]
(
	[StudentMajorId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[MajorName] VARCHAR(64) NOT NULL,
	[MajorCode] VARCHAR(6) NOT NULL,
    [IsSystems] BIT NOT NULL,
	[IsEducation] BIT NOT NULL, 
    [IsGraduate] BIT NOT NULL,
	[CreationDate] DATETIME2(7) NOT NULL DEFAULT (sysdatetime()),
	[ModifiedDate] DATETIME2(7) NOT NULL DEFAULT (sysdatetime())
)

GO
CREATE UNIQUE INDEX [IX_StudentMajors_Column] ON [Patron].[StudentMajors] ([MajorName],[MajorCode])
