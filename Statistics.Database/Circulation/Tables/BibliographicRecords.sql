CREATE TABLE [Circulation].[BibliographicRecords] (
    [BibliographicRecordID] INT  NOT NULL IDENTITY,
    [OclcNumber]            INT    NULL,
    [Title]                 NVARCHAR (MAX)   NOT NULL,
    [MaterialFormat]        NVARCHAR (32)    NOT NULL,
    [Author]                NVARCHAR (MAX)   NULL,
    [RunDate]               DATE             NOT NULL,
    [CreationDate]          DATETIME2 (7)    CONSTRAINT [DF_BibliographicRecords_CreationDate] DEFAULT (sysdatetime()) NOT NULL,
    [ModifiedDate]          DATETIME2 (7)    CONSTRAINT [DF_BibliographicRecords_ModifiedDate] DEFAULT (sysdatetime()) NOT NULL,
    CONSTRAINT [PK_BibliographicRecords] PRIMARY KEY CLUSTERED ([BibliographicRecordID] ASC)
);

