CREATE TABLE [EZProxy].[EZProxyLog]
(
	[IP] NVARCHAR(15) NOT NULL , 
    [Username] NCHAR(15) NULL, 
    [DateTime] DATETIME NOT NULL, 
    [Request] NVARCHAR(MAX) NOT NULL, 
    [HTTPCode] INT NOT NULL, 
	[BytesTransferred] INT NOT NULL,
    [Referer] NVARCHAR(MAX) NULL, 
    [UserAgent] NVARCHAR(MAX) NULL
)
