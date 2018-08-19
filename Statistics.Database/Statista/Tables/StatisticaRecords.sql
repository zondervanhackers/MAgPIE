CREATE TABLE [Statista].[StatisticaRecords]
(
	[ID] INT NULL, 
    [Date] DATETIME2 NOT NULL,
	[ContentType] NVARCHAR(30) NULL, 
    [MainIndustry] NVARCHAR(50) NULL, 
    [Title] NVARCHAR(150) NOT NULL, 
    [TypeofAccess] NVARCHAR(30) NOT NULL, 
    [Content] NVARCHAR(30) NULL, 
    [Subtype] NVARCHAR(50) NULL,
    PRIMARY KEY ([Date], [Title], [TypeofAccess])
)
