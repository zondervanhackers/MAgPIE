CREATE TABLE [Circulation].[Items] (
    [ItemID]                INT  NOT NULL IDENTITY,
    [BibliographicRecordID] int NOT NULL,
    [Barcode]               NVARCHAR (128)   NOT NULL,
    [Cost]                  NVARCHAR (MAX)   NULL,
    [LastInventoriedDate]   DATETIME2 (7)    NULL,
    [DeletedDate]           DATETIME2 (7)    NULL,
    [ItemType]              NVARCHAR (16)    NOT NULL,
    [RunDate]               DATE             NOT NULL,
    [CreationDate]          DATETIME2 (7)    CONSTRAINT [DF_Items_CreationDate] DEFAULT (sysdatetime()) NOT NULL,
    [ModifiedDate]          DATETIME2 (7)    CONSTRAINT [DF_Items_ModifiedDate] DEFAULT (sysdatetime()) NOT NULL,
    CONSTRAINT [PK_Items] PRIMARY KEY CLUSTERED ([ItemID] ASC),
    CONSTRAINT [FK_Items_BibliographicRecords] FOREIGN KEY ([BibliographicRecordID]) REFERENCES [Circulation].[BibliographicRecords] ([BibliographicRecordID])
);
GO
CREATE NONCLUSTERED INDEX ItemIndex
ON [Circulation].[Items] ([Barcode])
INCLUDE ([BibliographicRecordID])


