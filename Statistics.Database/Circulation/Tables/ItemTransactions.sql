CREATE TABLE [Circulation].[ItemTransactions] (
    [ItemTransactionID] INT NOT NULL IDENTITY,
	[ItemBarcode] nvarchar(128) NULL,
	[PatronDemographicID] INT NULL,
	[ItemLocationID] INT NULL,
	[TransactionType] nvarchar(16) NOT NULL,
	[UserBarcode] nvarchar(16) NULL,
	[LoanDueDate] datetime2(7) NOT NULL,
	[LoanCheckedOutDate] datetime2(7) NOT NULL,
	[InstitutionName] nvarchar(50) NOT NULL,
	[RunDate] date NOT NULL,
	[RecordDate] date NOT NULL,
	[CreationDate] datetime2(7) NOT NULL,
	[ModifiedDate] datetime2(7) NOT NULL, 
    CONSTRAINT [PK_ItemTransactions] PRIMARY KEY ([ItemTransactionID]),
    CONSTRAINT [FK_ItemTransactions_ItemLocations] FOREIGN KEY ([ItemLocationID]) REFERENCES [Circulation].[ItemLocations]([ItemLocationID]),
    CONSTRAINT [UK_ItemTransactions] UNIQUE ([ItemBarcode], [UserBarcode], [TransactionType], [PatronDemographicID], [ItemLocationID], [LoanCheckedOutDate], [LoanDueDate], [RecordDate])
);

