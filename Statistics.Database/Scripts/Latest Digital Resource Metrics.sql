
SELECT R.ResourceName, R.ResourceType, R.ResourcePlatform, RR.RunDate, RM.MetricType, RM.MetricValue
FROM [Statistics].[Digital].[Resources] AS R
INNER JOIN [Statistics].[Digital].[ResourceRecords] aS RR
ON R.ResourceId = RR.ResourceId
INNER JOIN [Statistics].[Digital].[ResourceRecordMetrics] AS RM
ON RR.ResourceRecordId = RM.ResourceRecordId
ORDER BY R.CreationDate DESC
GO
