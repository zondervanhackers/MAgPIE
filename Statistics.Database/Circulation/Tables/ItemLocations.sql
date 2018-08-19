CREATE TABLE [Circulation].[ItemLocations] (
    [ItemLocationID]   INT				NOT NULL IDENTITY,
    [ItemID]           INT NOT NULL,
    [CallNumber]       NVARCHAR (64)    NULL,
    [ShelvingLocation] NVARCHAR (64)    NULL,
    [CurrentStatus]    NVARCHAR (64)    NOT NULL,
    [Description]      NVARCHAR (256)   NULL,
    [CreationDate]     DATETIME2 (7)    CONSTRAINT [DF_ItemLocations_CreationDate] DEFAULT (sysdatetime()) NOT NULL,
    [ModifiedDate]     DATETIME2 (7)    CONSTRAINT [DF_ItemLocations_ModifiedDate] DEFAULT (sysdatetime()) NOT NULL,
    CONSTRAINT [FK_ItemLocations_Items] FOREIGN KEY ([ItemID]) REFERENCES [Circulation].[Items] ([ItemID]), 
    CONSTRAINT [PK_ItemLocations] PRIMARY KEY ([ItemLocationID])
);

