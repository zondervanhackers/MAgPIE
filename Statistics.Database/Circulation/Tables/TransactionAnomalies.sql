
CREATE TABLE [Circulation].[TransactionAnomalies]
(
	[ItemBarcode] NVARCHAR(16) NULL, 
    [UserBarcode] NVARCHAR(16) NULL, 
    [LoanDueDate] DATETIME2 NULL, 
    [LoanCheckedOutDate] DATETIME2 NULL, 
    [RecordDate] DATE NOT NULL, 
    [TransactionType] NVARCHAR(16) NOT NULL, 
    [InstitutionName] NVARCHAR(MAX) NOT NULL, 
    [CountFound] SMALLINT NOT NULL, 
    [RunDate] DATE NOT NULL
)