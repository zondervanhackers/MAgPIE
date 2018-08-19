CREATE TABLE [Digital].[ResourceRecordMetrics]
(
	[ResourceRecordId] BIGINT NOT NULL , 
    [MetricType] VARCHAR(32) NOT NULL, 
    [MetricValue] INT NOT NULL, 
    CONSTRAINT [FK_ResourceRecordMetrics_ResourceRecords] FOREIGN KEY ([ResourceRecordId]) REFERENCES [Digital].[ResourceRecords]([ResourceRecordId]), 
    PRIMARY KEY ([ResourceRecordId], [MetricType])
)
