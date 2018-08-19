CREATE TABLE [Circulation].[ItemLocationDates] (
    [ItemLocationID] INT NOT NULL,
    [RunDate]        DATE             NOT NULL,
    CONSTRAINT [PK_ItemLocationDates] PRIMARY KEY CLUSTERED ([ItemLocationID] ASC, [RunDate] ASC),
    CONSTRAINT [FK_ItemLocationDates_ItemLocations] FOREIGN KEY ([ItemLocationID]) REFERENCES [Circulation].[ItemLocations] ([ItemLocationID])
);

