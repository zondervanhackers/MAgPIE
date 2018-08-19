using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ZondervanLibrary.Statistics.Entities
{
    public partial class StatisticsDataContext : IStatisticsDataContext
    {
        public Action<String> LogMessage { get; set; }
        partial void OnCreated()
        {
            Connection.Open();
            CommandTimeout = 3600;
        }

        private string previousTables = "";

        /// <summary>
        /// Create a table and load rows of data into it.
        /// </summary>
        /// <param name="createTableSql">The Query that creates the table.</param>
        /// <param name="tableName">The table Name to be created.</param>
        /// <param name="dataReader">The Data Reader containing the data.</param>
        /// <param name="timeout">Number of seconds allowed before it times out.</param>
        public void BulkCopy(string createTableSql, string tableName, IDataReader dataReader, int timeout = 0)
        {
            ExecuteCommand(string.Format(createTableSql, tableName));

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(Connection as SqlConnection, SqlBulkCopyOptions.TableLock, null))
            {
                bulkCopy.DestinationTableName = tableName;
                bulkCopy.EnableStreaming = true;
                bulkCopy.BatchSize = 10000;
                bulkCopy.BulkCopyTimeout = timeout;

                try
                {
                    previousTables += ";" + tableName;
                    bulkCopy.WriteToServer(dataReader);
                }
                catch (Exception ex)
                {
                    if (previousTables != "")
                    {
                        DeleteTemporaryTables(previousTables.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
                    }

                    throw new InvalidOperationException($"Failed to copy {tableName} to {Connection.Database} when row was encountered because {ex.Message}", ex);
                }
            }
        }

        public void BulkImportCounterTransactions(IDataReader recordsDataReader, IDataReader identifierDataReader, IDataReader metricDataReader)
        {
            string shortDate = recordsDataReader.GetDateTime(7).ToShortDateString();
            String tempItemTableName = string.Format("[Digital].[TransactionImport-{1}{0}]", Guid.NewGuid(), shortDate);
            BulkCopy(
                @"CREATE TABLE {0}(
                    [ResourceId] [int] NULL,
                    [ResourceRecordId] [bigint] NULL,
                    [ParentResourceRecordId] [bigint] NULL,
                    [ItemIndex] [int] NOT NULL,
                    [ItemName] [nvarchar](max) NOT NULL,
                    [ItemPlatform] [nvarchar](100) NULL,
                    [ItemType] [varchar](32) NOT NULL,
                    [RunDate] [date] NOT NULL
                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]", tempItemTableName, recordsDataReader);

            String tempIdentifierTableName = string.Format("[Digital].[TransactionIdentifierImport-{1}{0}]", Guid.NewGuid(), shortDate);

            BulkCopy(
                @"CREATE TABLE {0}(
                    [ItemIndex] [int] NOT NULL,
                    [IdentifierType] [varchar](32) NOT NULL,
                    [IdentifierValue] [nvarchar](64) NOT NULL,
                    [ItemType] [varchar](32) NOT NULL
                ) ON [PRIMARY]", tempIdentifierTableName, identifierDataReader);

            String tempMetricTableName = string.Format("[Digital].[TransactionMetricImport-{1}{0}]", Guid.NewGuid(), shortDate);

            BulkCopy(
                @"CREATE TABLE {0}(
                    [ItemIndex] [int] NOT NULL,
                    [MetricType] [varchar](32) NOT NULL,
                    [MetricValue] [nvarchar](64) NOT NULL
                ) ON [PRIMARY]", tempMetricTableName, metricDataReader);

            String identifierTransitionTableName = $"[Digital].[TransactionIdentifierTemp-{Guid.NewGuid()}]";

            String updateTableQuery = $@"SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

                BEGIN TRANSACTION ImportCounterTransactions

                BEGIN TRY

                CREATE TABLE {identifierTransitionTableName}
                (
	                ResourceId int,
	                ItemIndex int,
		            ResourcePlatform varchar(MAX)
                )
                
                /* Connect Resources with longer identifiers first */
                INSERT INTO {identifierTransitionTableName}
                SELECT RI.ResourceId, I.ItemIndex, Ri.ResourcePlatform
                FROM (
	                SELECT 
		                t1.ItemIndex,
		                (SELECT R.ItemPlatform FROM {tempItemTableName} AS R WHERE R.ItemIndex = t1.ItemIndex) AS [ResourcePlatform],
		                STUFF((
			                SELECT  ', ' + t2.IdentifierType, ', ' + t2.IdentifierValue
			                FROM {tempIdentifierTableName} t2
			                WHERE t2.ItemIndex = t1.ItemIndex
                            ORDER BY t2.IdentifierType, t2.IdentifierValue
			                FOR XML PATH ('')), 1 , 2, ''
		                ) AS PhoneNumbers
	                FROM {tempIdentifierTableName} t1
	                GROUP BY t1.ItemIndex
                ) AS I
                INNER JOIN (SELECT 
		                t1.ResourceId,
		                (SELECT R.ResourcePlatform FROM [Digital].[Resources] AS R WHERE R.ResourceId = t1.ResourceId) AS [ResourcePlatform],
		                STUFF((
			                SELECT  ', ' + t2.IdentifierType, ', ' + t2.IdentifierValue
			                FROM [Digital].[ResourceIdentifiers] t2
			                WHERE t2.ResourceId = t1.ResourceId
                            ORDER BY t2.IdentifierType, t2.IdentifierValue
			                FOR XML PATH ('')), 1 , 2, ''
		                ) AS Identifiers
	                FROM [Digital].[ResourceIdentifiers] t1
	                GROUP BY t1.ResourceId) AS RI
                ON RI.Identifiers = I.PhoneNumbers

                /* Update TempTable with identifier ResourceID */
                UPDATE A
                SET ResourceId = (SELECT ResourceId FROM {identifierTransitionTableName} AS B WHERE A.ItemIndex = B.ItemIndex AND A.ItemPlatform = B.ResourcePlatform)
                FROM {tempItemTableName} AS A

                /* Update ResourceID when a single match is found */
                UPDATE A
                SET ResourceId = 
	                (SELECT 
	                CASE when COUNT(R.ResourceID) = 1 then MAX(R.ResourceID)
		                 --when COUNT(R.ResourceID) > 1 then Digital.IdentifierMatch(A.ItemName, R)
                    end
	                FROM (SELECT C.ResourceId, C.ResourceName  FROM [Digital].[Resources] AS C 
		                JOIN (SELECT * FROM [Digital].[ResourceIdentifiers] WHERE EXISTS (SELECT DISTINCT IdentifierType, IdentifierValue FROM [Digital].[ResourceIdentifiers])) AS D 
		                ON D.ResourceId = C.ResourceId 
		                WHERE C.ResourceType = A.ItemType AND D.IdentifierType = B.IdentifierType AND D.IdentifierValue = B.IdentifierValue AND D.ResourceType = C.ResourceType AND (C.ResourcePlatform = A.ItemPlatform OR (C.ResourcePlatform IS NULL AND A.ItemPlatform IS NULL))) AS R)
                FROM {tempItemTableName} AS A
                JOIN {tempIdentifierTableName} AS B ON A.ResourceId IS NULL AND B.ItemIndex = A.ItemIndex

                /* Update ResourceRecords that already exist */
                UPDATE A
                SET ResourceRecordId = (SELECT B.ResourceRecordId FROM [Digital].[ResourceRecords] AS B WHERE B.ResourceId = A.ResourceId AND B.RunDate = A.RunDate)
                FROM {tempItemTableName} AS A
                WHERE A.ResourceId IS NOT NULL

                /* Create new ResourceId for new Resources */
                UPDATE A
                SET ResourceId = B.ResourceId
                FROM {tempItemTableName} AS A
                JOIN (
	                SELECT C.ItemIndex, (RANK() OVER (ORDER BY C.ItemIndex)) + (SELECT ISNULL(MAX(D.ResourceId), 0) FROM [Digital].[Resources] AS D) AS ResourceId
	                FROM {tempItemTableName} AS C
	                WHERE C.ResourceId IS NULL
                ) AS B ON A.ItemIndex = B.ItemIndex
                WHERE A.ResourceId IS NULL

                /* Create new ResourceRecordID for new ResourceRecords */
                UPDATE A
                SET ResourceRecordId = B.ResourceRecordId
                FROM {tempItemTableName} AS A
                JOIN (
	                SELECT C.ItemIndex, (RANK() OVER (ORDER BY C.ItemIndex)) + (SELECT ISNULL(MAX(D.ResourceRecordId), 0) FROM [Digital].[ResourceRecords] AS D) AS ResourceRecordId
	                FROM {tempItemTableName} AS C
	                WHERE C.ResourceRecordId IS NULL
                ) AS B ON B.ItemIndex = A.ItemIndex
                WHERE A.ResourceRecordId IS NULL

                /* Update ParentResourceRecordID */
                UPDATE A
                SET ParentResourceRecordId = B.ResourceRecordId
                FROM {tempItemTableName} AS A
                JOIN {tempItemTableName} AS B ON B.ItemName = A.ItemPlatform
	                WHERE (A.ItemType = 'Database' AND B.ItemType = 'Vendor') OR (A.ItemType != 'Database' AND A.ItemType != 'Vendor' AND (B.ItemType = 'Database' OR B.ItemType = 'Vendor'))
                --WHERE (A.ItemType = 'Database' AND B.ItemType = 'Vendor') OR (A.ItemType != 'Database' AND A.ItemType != 'Vendor' AND B.ItemType = 'Database') OR (A.ItemType != 'Database' AND A.ItemType != 'Vendor' AND B.ItemType = 'Vendor')

                /* Insert new Resources */
                INSERT INTO [Digital].[Resources] (ResourceId, ResourceName, ResourceType, ResourcePlatform)
                SELECT A.ResourceId, A.ItemName, A.ItemType, A.ItemPlatform
                FROM {tempItemTableName} AS A
                LEFT JOIN [Digital].[Resources] AS B ON B.ResourceId = A.ResourceId
                WHERE B.ResourceId IS NULL

                /* Insert new Identifiers */
                INSERT INTO [Digital].[ResourceIdentifiers] (ResourceId, IdentifierType, IdentifierValue, ResourceType)
                SELECT B.ResourceId, A.IdentifierType, A.IdentifierValue, A.ItemType
                FROM {tempIdentifierTableName} AS A
                JOIN {tempItemTableName} AS B ON B.ItemIndex = A.ItemIndex
                LEFT JOIN [Digital].[ResourceIdentifiers] AS C ON C.ResourceId = B.ResourceId AND C.IdentifierType = A.IdentifierType
                WHERE C.ResourceId IS NULL

                /* Merge ResourceRecords */
                MERGE [Digital].[ResourceRecords] AS D
                USING {tempItemTableName} AS S
                ON (D.ResourceRecordId = S.ResourceRecordId)
                WHEN NOT MATCHED THEN
	                INSERT (ResourceRecordId, ParentResourceRecordId, ResourceId, RunDate) VALUES (S.ResourceRecordId, S.ParentResourceRecordId, S.ResourceId, S.RunDate)
                WHEN MATCHED THEN
	                UPDATE
	                SET D.ModifiedDate = sysdatetime();

                /* Merge ResourceMetrics */
                MERGE [Digital].[ResourceRecordMetrics] AS D
                USING (
	                SELECT B.ResourceRecordId, A.MetricType, A.MetricValue
	                FROM {tempMetricTableName} AS A
	                JOIN {tempItemTableName} AS B ON B.ItemIndex = A.ItemIndex
                ) AS S
                ON (S.ResourceRecordId = D.ResourceRecordId AND S.MetricType = D.MetricType)
                WHEN NOT MATCHED THEN
	                INSERT (ResourceRecordId, MetricType, MetricValue) VALUES (S.ResourceRecordId, S.MetricType, S.MetricValue)
                WHEN MATCHED THEN
	                UPDATE
	                SET D.MetricValue = S.MetricValue;

                --Drop all temporary tables
                DROP TABLE {tempIdentifierTableName};
                DROP TABLE {tempItemTableName};
                DROP TABLE {tempMetricTableName};
                DROP TABLE {identifierTransitionTableName};
									   

                COMMIT TRANSACTION ImportCounterTransactions;

                END TRY
                BEGIN CATCH

                    ROLLBACK TRANSACTION ImportCounterTransactions;

                    THROW;

                END CATCH";

            int retries = 0;
            while (retries < 5)
            {
                try
                {
                    retries++;

                    Int32 rowsModified = ExecuteCommand(updateTableQuery);

                    HandleUnmodifiedDatabase(rowsModified, new[] { tempItemTableName, tempIdentifierTableName, tempMetricTableName, identifierTransitionTableName });

                    break;
                }
                catch (SqlException ex) when (ex.Number == 1205)
                {
                    if (retries >= 5)
                        throw;

                    Task.Delay(TimeSpan.FromSeconds(30)).Wait();
                }
            }
        }

        private void HandleUnmodifiedDatabase(int rowsModified, string[] temporarytables)
        {
            if (rowsModified == 0)
            {
                throw new InvalidOperationException($"Database was not modified.\r\nTables {String.Join(", ", temporarytables)} were left in the database for debugging purposes.");
            }
            else
            {
                DeleteTemporaryTables(temporarytables);
            }
        }

        public void BulkImportStatista(IDataReader statistaDataReader)
        {
            String tempStatistaTableName = $"[Statista].[StatistaImport-{Guid.NewGuid()}]";
            BulkCopy(
                @"CREATE TABLE {0}(
                    [ID] INT NULL,
                    [Date] DATETIME2 NOT NULL,
	                [ContentType] varchar(30) NULL,
                    [MainIndustry] varchar(50) NULL,
                    [Title] varchar(150) NOT NULL,
                    [TypeofAccess] varchar(30) NOT NULL,
                    [Content] varchar(30) NULL,
                    [Subtype] varchar(50) NULL
                )", tempStatistaTableName, statistaDataReader);

            String updateTableQuery = $@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

                BEGIN TRANSACTION ImportStatistaOperation

                BEGIN TRY

                    MERGE [Statista].[StatisticaRecords] AS T
                    USING (
	                    SELECT Date, Title, TypeofAccess, MAX(ID) as ID, MAX(ContentType) AS ContentType, MAX(MainIndustry) AS MainIndustry, MAX(Content) AS Content, MAX(Subtype) AS Subtype
	                    FROM {tempStatistaTableName}
	                    GROUP BY Date, Title, TypeofAccess
                    ) AS S
                    ON (S.Date = T.Date AND S.Title = T.Title AND S.TypeofAccess = T.TypeofAccess)
                    WHEN NOT MATCHED THEN
	                    INSERT(ID, Date, ContentType, MainIndustry, Title, TypeofAccess, Content, Subtype)
	                    VALUES(S.ID, S.Date, S.ContentType, S.MainIndustry, S.Title, S.TypeofAccess, S.Content, S.Subtype);

                    DROP TABLE {tempStatistaTableName};

                    COMMIT TRANSACTION ImportStatistaOperation;

                END TRY
                BEGIN CATCH

                    ROLLBACK TRANSACTION ImportStatistaOperation;

                    THROW;

                END CATCH";

            try
            {
                Int32 rowsModified = ExecuteCommand(updateTableQuery);

                HandleUnmodifiedDatabase(rowsModified, new[] { tempStatistaTableName });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException);
                    Console.WriteLine(ex.InnerException.Message);
                }
                throw;
            }
        }

        public void BulkImportDemographics(IDataReader patronDataReader, IDataReader majorDataReader, IDataReader residenceDataReader)
        {
            String tempPatronTableName = $"[Patron].[PatronImport-{Guid.NewGuid()}]";
            String tempPatronResidenceTableName = $"[Patron].[PatronResidenceImport-{Guid.NewGuid()}]";
            String tempPatronMajorTableName = $"[Patron].[PatronMajorImport-{Guid.NewGuid()}]";

            BulkCopy(
                @"CREATE TABLE {0} (
                    [ItemIndex] int NOT NULL PRIMARY KEY,
                    [PatronId] int NULL,
                    [PatronRecordId] int NULL,
                    [StudentRecordId] int NULL,
                    [EffectiveStartDate] datetime2(7) NULL,
                    [EffectiveEndDate] datetime2(7) NULL,
                    [Barcode] char(14) NOT NULL,
                    [Gender] varchar(8) NOT NULL, 
                    [StudentLevel] varchar(16) NOT NULL,
                    [StudentType] varchar(64) NOT NULL,
                    [StudentClass] varchar(16) NULL,
                    [CumulativeGpa] decimal(3,2) NULL
                )", tempPatronTableName, patronDataReader);

            BulkCopy(
                @"CREATE TABLE {0} (
                    [ItemIndex] int NOT NULL,
                    [MajorName] varchar(64) NOT NULL,
                    [MajorCode] varchar(6) NOT NULL,
                    [MajorType] varchar(8) NOT NULL,
                    [IsSystems] bit NOT NULL,
                    [IsEducation] bit NOT NULL,
                    [IsGraduate] bit NOT NULL
                )", tempPatronMajorTableName, majorDataReader);

            BulkCopy(
                @"CREATE TABLE {0} (
                    [ItemIndex] int NOT NULL,
                    [ResidenceName] varchar(32) NOT NULL,
                    [ResidenceCategory] varchar(16) NOT NULL
                )", tempPatronResidenceTableName, residenceDataReader);

            Guid guid = Guid.NewGuid();
            String updateTableQuery = $@"
                BEGIN TRANSACTION ImportDemographicsOperation

                BEGIN TRY

                    /* Match to existing PatronIds */
                    UPDATE A
                    SET PatronId = B.PatronId
                    FROM {tempPatronTableName} AS A
                    JOIN [Patron].[Patrons] AS B ON B.Barcode = A.Barcode;

                    /* Assign new PatronIds */
                    UPDATE A
                    SET PatronId = B.PatronId
                    FROM {tempPatronTableName} AS A
                    JOIN (
                        SELECT C.Barcode, (RANK() OVER (ORDER BY C.Barcode)) + (SELECT ISNULL(MAX(D.PatronId), 0) FROM [Patron].[Patrons] AS D) AS PatronId
                        FROM {tempPatronTableName} AS C
                        WHERE C.PatronId IS NULL
                        GROUP BY Barcode
                    ) AS B ON B.Barcode = A.Barcode;

                    CREATE INDEX [IX_{guid}_PatronId] ON {tempPatronTableName} (PatronId);

                    /* Match to existing PatronRecordIds */
                    UPDATE A
                    SET PatronRecordId = B.PatronRecordId
                    FROM {tempPatronTableName} AS A
                    JOIN [Patron].[PatronRecords] AS B ON B.PatronId = A.PatronId AND B.EffectiveStartDate = A.EffectiveStartDate AND B.EffectiveEndDate = A.EffectiveEndDate;

                    /* Assign new PatronRecordIds */
                    UPDATE A
                    SET PatronRecordId = B.PatronRecordId
                    FROM {tempPatronTableName} AS A
                    JOIN (
                        SELECT C.ItemIndex AS ItemIndex, (RANK() OVER (ORDER BY C.ItemIndex)) + (SELECT ISNULL(MAX(D.PatronRecordId), 0) FROM [Patron].[PatronRecords] AS D) AS PatronRecordId
                        FROM {tempPatronTableName} AS C
                        WHERE C.PatronRecordId IS NULL
                    ) AS B ON B.ItemIndex = A.ItemIndex;                  

                    CREATE INDEX [IX_{guid}_PatronRecordId] ON {tempPatronTableName} (PatronRecordId);

                    /* Match to existing StudentRecordIds */
                    UPDATE A
                    SET StudentRecordId = B.StudentRecordId
                    FROM {tempPatronTableName} AS A
                    JOIN [Patron].[StudentRecords] AS B ON B.PatronRecordId = A.PatronRecordId;    

                    /* Assign new StudentRecordIds */
                    UPDATE A
                    SET StudentRecordId = B.StudentRecordId
                    FROM {tempPatronTableName} AS A
                    JOIN (
                        SELECT C.ItemIndex AS [ItemIndex], (RANK() OVER (ORDER BY C.ItemIndex)) + (SELECT ISNULL(MAX(D.StudentRecordId), 0) FROM [Patron].[StudentRecords] AS D) AS StudentRecordId
                        FROM {tempPatronTableName} AS C
                        WHERE C.StudentRecordId IS NULL
                    ) AS B ON B.ItemIndex = A.ItemIndex;

                    CREATE INDEX [IX_{guid}_StudentRecordId] ON {tempPatronTableName} (StudentRecordId);

                    /* Merge Patrons */
                    MERGE [Patron].[Patrons] AS D
                    USING (
                        SELECT A.PatronId, MAX(A.Barcode) AS Barcode, MAX(A.Gender) AS Gender
                        FROM {tempPatronTableName} AS A
                        GROUP BY A.PatronId
                    )AS S
                    ON (D.PatronId = S.PatronId)
                    WHEN NOT MATCHED THEN
                        INSERT (PatronId, Barcode, Gender) VALUES (S.PatronId, S.Barcode, S.Gender)
                    WHEN MATCHED THEN
                        UPDATE SET
                            Gender = S.Gender,
                            ModifiedDate = sysdatetime();

                    /* Merge PatronRecords */
                    MERGE [Patron].[PatronRecords] AS D
                    USING {tempPatronTableName} AS S
                    ON (D.PatronRecordId = S.PatronRecordId)
                    WHEN NOT MATCHED THEN
                        INSERT (PatronRecordId, PatronId, EffectiveStartDate, EffectiveEndDate) VALUES (S.PatronRecordId, S.PatronId, S.EffectiveStartDate, S.EffectiveEndDate)
                    WHEN MATCHED THEN
                        UPDATE SET
                            ModifiedDate = sysdatetime();
                                
                    /* Merge StudentRecords */
                    MERGE [Patron].[StudentRecords] AS D
                    USING {tempPatronTableName} AS S
                    ON (D.StudentRecordId = S.StudentRecordId)
                    WHEN NOT MATCHED THEN
                        INSERT (StudentRecordId, PatronRecordId, StudentLevel, StudentType, StudentClass, CumulativeGpa) VALUES (S.StudentRecordId, S.PatronRecordId, S.StudentLevel, S.StudentType, S.StudentClass, S.CumulativeGpa)
                    WHEN MATCHED THEN
                        UPDATE SET
                            StudentLevel = S.StudentLevel,
                            StudentType = S.StudentType,
                            StudentClass = S.StudentClass,
                            CumulativeGpa = S.CumulativeGpa,
                            ModifiedDate = sysdatetime();
                                        
                    /* Merge StudentMajors */
                    MERGE [Patron].[StudentMajors] AS D
                    USING (
                        SELECT MajorName,
			                    MAX(MajorCode) AS MajorCode,
                                CAST(MAX(CAST(IsSystems AS int)) AS bit) AS IsSystems,
                                CAST(MAX(CAST(IsEducation AS int)) AS bit) AS IsEducation,
                                CAST(MAX(CAST(IsGraduate AS int)) AS bit) AS IsGraduate
                        FROM {tempPatronMajorTableName} AS A
                        GROUP BY MajorName, MajorCode
                    ) AS S
                    ON (S.MajorName = D.MajorName AND S.MajorCode = D.MajorCode)
                    WHEN NOT MATCHED THEN
                        INSERT (MajorName, MajorCode, IsSystems, IsEducation, IsGraduate) VALUES (S.MajorName, S.MajorCode, S.IsSystems, S.IsEducation, S.IsGraduate)
                    WHEN MATCHED THEN
                        UPDATE SET
                            IsSystems = S.IsSystems,
                            IsEducation = S.IsEducation,
                            IsGraduate = S.IsGraduate,
                            ModifiedDate = sysdatetime();

                    /* Merge StudentMajorRecords */
                    MERGE [Patron].[StudentMajorRecords] AS D
                    USING (
                        SELECT A.StudentRecordId, B.MajorType, C.StudentMajorId
                        FROM {tempPatronTableName} AS A
                        JOIN {tempPatronMajorTableName} AS B ON B.ItemIndex = A.ItemIndex
                        JOIN [Patron].[StudentMajors] AS C ON C.MajorName = B.MajorName AND C.MajorCode = B.MajorCode
                    ) AS S
                    ON (S.StudentRecordId = D.StudentRecordId AND S.MajorType = D.MajorType)
                    WHEN NOT MATCHED THEN
                        INSERT (StudentRecordId, MajorType, StudentMajorId) VALUES (S.StudentRecordId, S.MajorType, S.StudentMajorId)
                    WHEN MATCHED THEN
                        UPDATE SET
                            StudentMajorId = S.StudentMajorId,
                            ModifiedDate = sysdatetime();

                    /* Merge StudentResidences */
                    MERGE [Patron].[StudentResidences] AS D
                    USING (
                        SELECT ResidenceName,
                                MAX(ResidenceCategory) AS ResidenceCategory
                        FROM {tempPatronResidenceTableName} AS A
                        GROUP BY ResidenceName
                    ) AS S
                    ON (S.ResidenceName = D.ResidenceName)
                    WHEN NOT MATCHED THEN
                        INSERT (ResidenceName, ResidenceCategory) VALUES (S.ResidenceName, S.ResidenceCategory)
                    WHEN MATCHED THEN
                        UPDATE SET
                            ResidenceCategory = S.ResidenceCategory,
                            ModifiedDate = sysdatetime();

                    /* Merge StudentResidenceRecords */
                    MERGE [Patron].[StudentResidenceRecords] AS D
                    USING (
                        SELECT B.StudentRecordId, C.StudentResidenceId
                        FROM {tempPatronResidenceTableName} AS A
                        JOIN {tempPatronTableName} AS B ON B.ItemIndex = A.ItemIndex
                        JOIN [Patron].[StudentResidences] AS C ON C.ResidenceName = A.ResidenceName
                    ) AS S
                    ON (S.StudentRecordId = D.StudentRecordId)
                    WHEN NOT MATCHED THEN
                        INSERT (StudentRecordId, StudentResidenceId) VALUES (S.StudentRecordId, S.StudentResidenceId)
                    WHEN MATCHED THEN
                        UPDATE SET
                            StudentResidenceId = S.StudentResidenceId,
                            ModifiedDate = sysdatetime();

                    /* Delete extra records (cascade will delete associate records) */
                    DELETE A
                    FROM [Patron].[PatronRecords] AS A
                    JOIN (
                        SELECT DISTINCT C.EffectiveStartDate, C.EffectiveEndDate
                        FROM {tempPatronTableName} AS C
                    ) AS B ON B.EffectiveStartDate = A.EffectiveStartDate AND B.EffectiveEndDate = A.EffectiveEndDate
                    LEFT JOIN {tempPatronTableName} AS D ON D.PatronRecordId = A.PatronRecordId AND D.EffectiveStartDate = A.EffectiveStartDate AND D.EffectiveEndDate = A.EffectiveEndDate
                    WHERE D.PatronRecordId IS NULL

                    /* Delete potential major extra records */
                    DELETE A
                    FROM [Patron].[StudentMajorRecords] AS A
                    JOIN {tempPatronTableName} AS B ON B.StudentRecordId = A.StudentRecordId
                    LEFT JOIN {tempPatronMajorTableName} AS C ON C.ItemIndex = B.ItemIndex AND C.MajorType = A.MajorType
                    WHERE C.ItemIndex IS NULL

                    DROP TABLE {tempPatronTableName};
                    DROP TABLE {tempPatronMajorTableName};
                    DROP TABLE {tempPatronResidenceTableName};

                    COMMIT TRANSACTION ImportDemographicsOperation;

                END TRY
                BEGIN CATCH

                    ROLLBACK TRANSACTION ImportDemographicsOperation;

                    THROW;

                END CATCH";

            try
            {
                Int32 rowsModified = ExecuteCommand(updateTableQuery);

                HandleUnmodifiedDatabase(rowsModified, new[] { tempPatronTableName, tempPatronMajorTableName, tempPatronResidenceTableName });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException == null) throw;
                Console.WriteLine(ex.InnerException);
                Console.WriteLine(ex.InnerException.Message);
                throw;
            }
        }

        public void BulkImportInventory(IDataReader dataReader)
        {
            String tempTableName = $"[Circulation].[InventoryImport-{Guid.NewGuid()}]";
            String transitionTableName = $"[Circulation].[InventoryImportT-{Guid.NewGuid()}]";

            BulkCopy(
                @"CREATE TABLE {0}(
	                [OclcNumber] [nvarchar](16) NULL,
	                [Title] [nvarchar](max) NULL,
	                [MaterialFormat] [nvarchar](32) NOT NULL,
	                [Author] [nvarchar](max) NULL,
	                [Barcode] [nvarchar](32) NULL,
	                [Cost] [nvarchar](255) NULL,
	                [LastInventoriedDate] [datetime2](7) NULL,
	                [DeletedDate] [datetime2](7) NULL,
	                [ItemType] [nvarchar](16) NOT NULL,
	                [CallNumber] [nvarchar](64) NULL,
	                [ShelvingLocation] [nvarchar](32) NULL,
	                [CurrentStatus] [nvarchar](16) NULL,
	                [Description] [nvarchar](max) NULL,
	                [RunDate] [date] NOT NULL,
                    [Anomalous] [bit] NOT NULL
                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]", tempTableName, dataReader, 180);

            String updateTableQuery = $@"BEGIN TRANSACTION ImportInventory

                BEGIN TRY

                    declare @Barcodes table (Barcode nvarchar(32));

                    MERGE [Circulation].[InventoryAnomalies] AS IA
                    USING (
                        SELECT I.OclcNumber, I.Barcode, I.RunDate, I.Title, I.MaterialFormat, I.Author, I.Cost, I.LastInventoriedDate, I.DeletedDate, I.ItemType, I.CallNumber, I.ShelvingLocation, I.CurrentStatus, I.Description
	                    FROM {tempTableName} AS I
	                    WHERE I.Anomalous = 1
                    ) AS result
                    ON ((IA.Title = result.Title OR (IA.Title IS NULL AND result.Title IS NULL)) AND (IA.Barcode = result.Barcode OR (IA.Barcode IS NULL AND result.Barcode IS NULL)))
                    WHEN NOT MATCHED THEN
	                    INSERT(OclcNumber, Title, MaterialFormat, Author, Barcode, Cost, LastInventoriedDate, DeletedDate, ItemType, CallNumber, ShelvingLocation, CurrentStatus, Description, RunDate, CountFound)
	                    VALUES(result.OclcNumber, result.Title, result.MaterialFormat, result.Author, result.Barcode, result.Cost, result.LastInventoriedDate, result.DeletedDate, result.ItemType, result.CallNumber, result.ShelvingLocation, result.CurrentStatus, result.Description, result.RunDate, 1)
                    WHEN MATCHED THEN
	                    UPDATE SET
	                    CountFound = CountFound + 1
                    OUTPUT inserted.Barcode INTO @Barcodes;

                    DELETE 
                    FROM {tempTableName}
                    WHERE Anomalous = 1;

                    -- Find previously stored fake OclcNumber for non-book records
                    UPDATE D
                    SET D.OclcNumber = BibliographicRecords.OclcNumber
                    FROM {tempTableName} AS D
                    JOIN [Circulation].[Items] ON Items.Barcode = D.Barcode
                    JOIN [Circulation].BibliographicRecords ON BibliographicRecords.BibliographicRecordID = Items.BibliographicRecordID;

                    -- Create new fake OclcNumber for new non-book records
                    UPDATE A
                    SET A.OclcNumber = B.OclcNumber
                    FROM {tempTableName} AS A
                    JOIN (
	                    SELECT Barcode, ((SELECT MIN(Value) FROM (VALUES ((SELECT MIN(OclcNumber) FROM [Circulation].[BibliographicRecords])), (0)) AS M(Value)) - (RANK() OVER (ORDER BY Barcode))) AS OclcNumber
	                    FROM {tempTableName} AS C
	                    WHERE C.OclcNumber IS NULL
                    ) AS B ON B.Barcode = A.Barcode
                    WHERE A.OclcNumber IS NULL;

                    -- Create table with proper index to speed up further calculations
                    CREATE TABLE {transitionTableName}(
	                    [OclcNumber] [nvarchar](16) NOT NULL,
	                    [Title] [nvarchar](max) NOT NULL,
	                    [MaterialFormat] [nvarchar](32) NOT NULL,
	                    [Author] [nvarchar](max) NULL,
	                    [Barcode] [nvarchar](32) NOT NULL,
	                    [Cost] [nvarchar](255) NULL,
	                    [LastInventoriedDate] [datetime2](7) NULL,
	                    [DeletedDate] [datetime2](7) NULL,
	                    [ItemType] [nvarchar](16) NOT NULL,
	                    [CallNumber] [nvarchar](64) NULL,
	                    [ShelvingLocation] [nvarchar](32) NULL,
	                    [CurrentStatus] [nvarchar](16) NULL,
	                    [Description] [nvarchar](max) NULL,
	                    [RunDate] [date] NOT NULL,
                        CONSTRAINT [PK_InventoryImport] PRIMARY KEY CLUSTERED ([OclcNumber] ASC, [Barcode] ASC, [RunDate] ASC));

                    -- Copy values from raw table into the indexed table
                    INSERT INTO {transitionTableName}
                    SELECT DISTINCT II.OclcNumber, II.Title, II.MaterialFormat, II.Author, II.Barcode, II.Cost, II.LastInventoriedDate, II.DeletedDate, II.ItemType, II.CallNumber, II.ShelvingLocation, II.CurrentStatus, II.Description, II.RunDate
                    FROM {tempTableName} AS II

                    -- Create/Update Bibliographic Records
                    MERGE [Circulation].[BibliographicRecords] AS T
                    USING (
	                    SELECT DISTINCT A.OclcNumber, A.Title, A.MaterialFormat, A.Author, A.RunDate
	                    FROM {transitionTableName} AS A
                    ) AS S
                    ON (S.OclcNumber = T.OclcNumber)
                    WHEN NOT MATCHED THEN
	                    INSERT (OclcNumber, Title, MaterialFormat, Author, RunDate) VALUES (S.OclcNumber, S.Title, S.MaterialFormat, S.Author, S.RunDate)
                    WHEN MATCHED AND S.RunDate > T.RunDate THEN
	                    UPDATE SET T.OclcNumber = S.OclcNumber,
	                    T.Title = S.Title,
	                    T.MaterialFormat = S.MaterialFormat,
	                    T.Author = S.Author,
	                    T.RunDate = S.RunDate,
	                    T.ModifiedDate = sysdatetime();

                    -- Create/Update Item Records
                    MERGE [Circulation].[Items] AS T
                    USING (
	                    SELECT B.BibliographicRecordID, A.Barcode, A.Cost, A.LastInventoriedDate, A.DeletedDate, A.ItemType, A.RunDate
	                    FROM {transitionTableName} AS A
	                    JOIN [Circulation].[BibliographicRecords] AS B ON B.OclcNumber = A.OclcNumber 
                    ) AS S
                    ON (S.Barcode = T.Barcode)
                    WHEN NOT MATCHED THEN
	                    INSERT (BibliographicRecordID, Barcode, Cost, LastInventoriedDate, DeletedDate, ItemType, RunDate) VALUES (S.BibliographicRecordID, S.Barcode, S.Cost, S.LastInventoriedDate, S.DeletedDate, S.ItemType, S.RunDate)
                    WHEN MATCHED AND S.RunDate > T.RunDate THEN
	                    UPDATE SET T.Cost = S.Cost,
		                T.LastInventoriedDate = S.LastInventoriedDate,
		                T.DeletedDate = S.DeletedDate,
		                T.ItemType = S.ItemType,
		                T.RunDate = S.RunDate,
		                T.ModifiedDate = sysdatetime();

                    -- Create/Update ItemLocation Records
                    MERGE [Circulation].[ItemLocations] AS T
                    USING (
                        SELECT B.ItemID, A.CallNumber, A.ShelvingLocation, A.CurrentStatus, A.Description, A.RunDate
                        FROM {transitionTableName} AS A
                        JOIN [Circulation].[Items] AS B ON B.Barcode = A.Barcode
                    ) AS S
                    ON (S.ItemId = T.ItemID AND ((S.CallNumber IS NULL AND T.CallNumber IS NULL) OR S.CallNumber = T.CallNumber) AND ((S.ShelvingLocation IS NULL AND T.ShelvingLocation IS NULL) OR S.ShelvingLocation = T.ShelvingLocation) AND S.CurrentStatus = T.CurrentStatus AND ((S.Description IS NULL AND T.Description IS NULL) OR S.Description = T.Description))
                    WHEN NOT MATCHED THEN
                        INSERT (ItemID, CallNumber, ShelvingLocation, CurrentStatus, Description) VALUES (S.ItemID, S.CallNumber, S.ShelvingLocation, S.CurrentStatus, S.Description)
                    WHEN MATCHED THEN
                        UPDATE SET T.CallNumber = S.CallNumber,
                                    T.ShelvingLocation = S.ShelvingLocation,
                                    T.CurrentStatus = S.CurrentStatus,
                                    T.Description = S.Description,
                                    T.ModifiedDate = sysdatetime();

                    -- Create/Update ItemLocationDate Records
                    MERGE [Circulation].[ItemLocationDates] AS T
                    USING (
                        SELECT C.ItemLocationID, A.RunDate
                        FROM {transitionTableName} AS A
                        JOIN [Circulation].[Items] AS B ON B.Barcode = A.Barcode
                        JOIN [Circulation].[ItemLocations] AS C ON C.ItemID = B.ItemID AND ((A.CallNumber IS NULL AND C.CallNumber IS NULL) OR A.CallNumber = C.CallNumber) AND ((A.ShelvingLocation IS NULL AND C.ShelvingLocation IS NULL) OR A.ShelvingLocation = C.ShelvingLocation) AND C.CurrentStatus = A.CurrentStatus AND ((A.Description IS NULL AND C.Description IS NULL) OR A.Description = C.Description)
                    ) AS S
                    ON (T.ItemLocationID = S.ItemLocationID AND T.RunDate = S.RunDate)
                    WHEN NOT MATCHED THEN
                        INSERT (ItemLocationID, RunDate) VALUES (S.ItemLocationID, S.RunDate);

                    -- Drop the temporary tables
                    DROP TABLE {tempTableName};
                    DROP TABLE {transitionTableName};

                    COMMIT TRANSACTION ImportInventory;

                END TRY

                BEGIN CATCH

                    ROLLBACK TRANSACTION ImportInventory;

                    THROW;

                END CATCH";

            try
            {
                Int32 rowsModified = ExecuteCommand(updateTableQuery);

                HandleUnmodifiedDatabase(rowsModified, new[] { tempTableName, transitionTableName });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException);
                    Console.WriteLine(ex.InnerException.Message);
                }
                throw;
            }
        }

        public void BulkImportTransactions(IDataReader dataReader)
        {
            String tempTableName = $"[Circulation].[TransactionsImport-{Guid.NewGuid()}]";
            String tempTransitionTableName = $"[Circulation].[TransactionImportTemp-{Guid.NewGuid()}]";

            BulkCopy(
                @"CREATE TABLE {0}(
	                [ItemBarcode] [nvarchar](128) NULL,
	                [UserBarcode] [nvarchar](16) NULL,
	                [LoanDueDate] [datetime2](7) NULL,
	                [LoanCheckedOutDate] [datetime2](7) NULL,
                    [RunDate] [date] NOT NULL,
                    [TransactionType] [nvarchar](16) NOT NULL,
                    [InstitutionName] [nvarchar](MAX) NOT NULL,
                    [RecordDate] [date] NOT NULL
                ) ON [PRIMARY]", tempTableName, dataReader);

            String updateTableQuery = $@"
                BEGIN TRANSACTION ImportTransactions

                BEGIN TRY
                
                    --Merge Duplicates and null ItemBarcodes into TransactionAnomalies
                    MERGE [Circulation].[TransactionAnomalies] AS T
                    USING (
	                    SELECT S.UserBarcode, S.ItemBarcode, S.RecordDate, S.LoanDueDate, S.LoanCheckedOutDate, S.TransactionType, S.InstitutionName, S.RunDate, COUNT(*) AS CountFound
	                    FROM {tempTableName} AS S
	                    GROUP BY S.UserBarcode, S.ItemBarcode, S.LoanDueDate, S.LoanCheckedOutDate, S.TransactionType, S.InstitutionName, S.RecordDate, S.RunDate
	                    HAVING COUNT(*) > 1 OR S.ItemBarcode IS NULL
                    ) AS source
                    ON ((T.ItemBarcode = source.ItemBarcode OR (T.ItemBarcode IS NULL AND source.ItemBarcode IS NULL)) AND (T.UserBarcode = source.UserBarcode OR (T.UserBarcode IS NULL AND source.UserBarcode IS NULL)) AND T.TransactionType = source.TransactionType AND T.RunDate = source.RunDate AND T.LoanCheckedOutDate = source.LoanCheckedOutDate AND T.LoanDueDate = source.LoanDueDate AND T.RecordDate = source.RecordDate)
                    WHEN NOT MATCHED THEN
	                    INSERT(UserBarcode, ItemBarcode, TransactionType, RecordDate, RunDate, LoanDueDate, LoanCheckedOutDate, InstitutionName, CountFound)
	                    VALUES(source.UserBarcode, source.ItemBarcode, source.TransactionType, source.RecordDate, source.RunDate, source.LoanDueDate, source.LoanCheckedOutDate, source.InstitutionName, source.CountFound)
                    WHEN MATCHED THEN
	                    UPDATE SET
		                    CountFound = source.CountFound;
                    
                    --Create Temp Table with that will store all valid rows.
	                CREATE TABLE {tempTransitionTableName}(
	                    [ItemBarcode] [nvarchar](128) NULL,
	                    [UserBarcode] [nvarchar](16) NULL,
	                    [LoanDueDate] [datetime2](7) NULL,
	                    [LoanCheckedOutDate] [datetime2](7) NULL,
                        [RunDate] [date] NOT NULL,
                        [TransactionType] [nvarchar](16) NOT NULL,
                        [InstitutionName] [nvarchar](MAX) NOT NULL,
                        [RecordDate] [date] NOT NULL
                    )

                    --Insert all rows without anomalies.
                    INSERT INTO {tempTransitionTableName}
                    SELECT S.ItemBarcode, S.UserBarcode, S.LoanDueDate, S.LoanCheckedOutDate, S.RunDate, S.TransactionType, S.InstitutionName, S.RecordDate
                    FROM {tempTableName} AS S
                    GROUP BY S.ItemBarcode, S.UserBarcode, S.LoanDueDate, S.LoanCheckedOutDate, S.TransactionType, S.InstitutionName, S.RecordDate, S.RunDate
                    HAVING COUNT(*) < 2 AND S.ItemBarcode IS NOT NULL

	                --Merge Transaction import into ItemTransactions linking Patron and Item Data.
                    MERGE [Circulation].[ItemTransactions] AS IT
                    USING (
                        SELECT ItemBarcode, PatronRecordId, MAX(IL.ItemLocationID) AS ItemLocationID, MAX(TransactionType) AS TransactionType, UserBarcode, MAX(LoanDueDate) AS LoanDueDate, MAX(LoanCheckedOutDate) AS LoanCheckedOutDate, B.RecordDate, MAX(B.RunDate) AS RunDate, MAX(InstitutionName) AS InstitutionName, SYSDATETIME() AS CreatedDate, SYSDATETIME() AS ModifiedDate
		                FROM (
			                SELECT C.PatronId, C.PatronRecordId, T.UserBarcode, T.ItemBarcode AS ItemBarcode, T.LoanCheckedOutDate, T.LoanDueDate, T.TransactionType, T.InstitutionName, T.RunDate, T.RecordDate, C.EffectiveStartDate, C.EffectiveEndDate
			                FROM (
				                SELECT P.PatronId, R.PatronRecordId, P.Barcode, R.EffectiveStartDate, R.EffectiveEndDate
				                FROM [Patron].[PatronRecords] AS R
				                INNER JOIN
				                [Patron].[Patrons] AS P
				                ON P.PatronId = R.PatronId
			                ) AS C
			                RIGHT JOIN
			                {tempTransitionTableName} AS T
			                ON (T.LoanCheckedOutDate BETWEEN C.EffectiveStartDate AND C.EffectiveEndDate) AND (T.UserBarcode = C.Barcode)
                        ) AS B
		                LEFT JOIN
		                [Circulation].[Items] AS I
		                ON (I.Barcode = B.ItemBarcode)
		                LEFT JOIN
		                [Circulation].[ItemLocations] AS IL
		                ON (I.ItemID = IL.ItemID)
		                GROUP BY ItemBarcode, UserBarcode, TransactionType, PatronRecordId, ItemLocationID, LoanCheckedOutDate, LoanDueDate, RecordDate
                    ) AS import
                    ON ((IT.ItemBarcode = import.ItemBarcode OR (IT.ItemBarcode IS NULL AND import.ItemBarcode IS NULL)) AND (IT.UserBarcode = import.UserBarcode OR (IT.UserBarcode IS NULL AND import.UserBarcode IS NULL)) AND IT.TransactionType = import.TransactionType AND (IT.PatronDemographicID = import.PatronRecordID OR (IT.PatronDemographicID IS NULL AND import.PatronRecordID IS NULL)) AND (IT.ItemLocationID = import.ItemLocationID OR (IT.ItemLocationID IS NULL AND import.ItemLocationID IS NULL)) AND IT.LoanCheckedOutDate = import.LoanCheckedOutDate AND IT.LoanDueDate = import.LoanDueDate AND IT.RecordDate = import.RecordDate)
                    WHEN NOT MATCHED THEN
	                    INSERT(ItemBarcode, PatronDemographicID, ItemLocationID, TransactionType, UserBarcode, LoanDueDate, LoanCheckedOutDate, InstitutionName, RunDate, RecordDate, CreationDate, ModifiedDate)
	                    VALUES(import.ItemBarcode, import.PatronRecordID, import.ItemLocationID, import.TransactionType, import.UserBarcode, import.LoanDueDate, import.LoanCheckedOutDate, import.InstitutionName, import.Rundate, import.RecordDate, import.CreatedDate, import.ModifiedDate)
                    WHEN MATCHED THEN
	                    UPDATE SET
		                    ModifiedDate = SYSDATETIME(),
			                RunDate = import.RunDate;

                    DROP TABLE {tempTableName};
                    DROP TABLE {tempTransitionTableName};

                    COMMIT TRANSACTION ImportTransactions;

                END TRY
                BEGIN CATCH

                    ROLLBACK TRANSACTION ImportTransactions;

                    THROW;
                    
                END CATCH";

            try
            {
                Int32 rowsModified = ExecuteCommand(updateTableQuery);

                HandleUnmodifiedDatabase(rowsModified, new[] { tempTableName, tempTransitionTableName });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException);
                    Console.WriteLine(ex.InnerException.Message);
                }
                throw;
            }
        }

        public void BulkImportEZProxyAudit(IDataReader dataReader)
        {
            String tempEZProxyTableName = $"[EZProxy].[EZProxyAuditImport-{Guid.NewGuid()}]";
            BulkCopy(
                @"CREATE TABLE {0}(
                    [DateTime] DATETIME NOT NULL,
                    [Event] NVARCHAR(50) NOT NULL, 
                    [IP] NVARCHAR(50) NULL,
                    [Username] NVARCHAR(100) NULL, 
                    [Session] NVARCHAR(50) NULL,
                    [Other] NVARCHAR(300) NULL,
                    [LineNumber] int NOT NULL
                )", tempEZProxyTableName, dataReader);

            String updateTableQuery = $@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

                BEGIN TRANSACTION ImportEZProxyAuditOperation

                BEGIN TRY

                    MERGE [EZProxy].[EZProxyAudit] AS T
                    USING (
	                    SELECT [DateTime], [Event], IP, Username, [Session], Other, LineNumber
	                    FROM {tempEZProxyTableName}
                    ) AS S
                    ON (T.[DateTime] = S.[DateTime] AND T.[Event] = S.[Event] AND T.LineNumber = S.LineNumber)
                    WHEN NOT MATCHED THEN
	                    INSERT(  [DateTime],   [Event],   IP,   Username,     [Session],   [Other], LineNumber, CreatedDate, ModifiedDate)
	                    VALUES(S.[DateTime], S.[Event], S.IP, S.[Username], S.[Session], S.[Other], LineNumber, SYSDATETIME(), SYSDATETIME())
                    WHEN MATCHED THEN
                        UPDATE SET T.ModifiedDate = SYSDATETIME();

                    DROP TABLE {tempEZProxyTableName};

                    COMMIT TRANSACTION ImportEZProxyAuditOperation;

                END TRY
                BEGIN CATCH

                    ROLLBACK TRANSACTION ImportEZProxyAuditOperation;

                    THROW;

                END CATCH";

            try
            {
                Int32 rowsModified = ExecuteCommand(updateTableQuery);

                HandleUnmodifiedDatabase(rowsModified, new[] { tempEZProxyTableName });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException);
                    Console.WriteLine(ex.InnerException.Message);
                }
                throw;
            }
        }

        public void BulkImportEZProxyLog(IDataReader dataReader)
        {
            String tempEZProxyTableName = $"[EZProxy].[EZProxyLogImport-{Guid.NewGuid()}]";
            BulkCopy(
                @"CREATE TABLE {0}(
                    [IP] NVARCHAR(15) NOT NULL, 
                    [Username] NCHAR(15) NULL, 
                    [DateTime] DATETIME NOT NULL,
                    [Request] NVARCHAR(MAX) NOT NULL, 
                    [HTTPCode] int NOT NULL, 
                    [BytesTransferred] int NOT NULL,
                    [Referer] NVARCHAR(MAX) NULL, 
                    [UserAgent] NVARCHAR(MAX) NULL, 
                )", tempEZProxyTableName, dataReader);

            String updateTableQuery = $@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

                BEGIN TRANSACTION ImportEZProxyLogOperation

                BEGIN TRY

                    INSERT INTO [EZProxy].[EZProxyLog]
                    SELECT [IP], [Username], [DateTime], [Request], [HTTPCode], [BytesTransferred], [Referer], [UserAgent]
                    FROM {tempEZProxyTableName}

                    COMMIT TRANSACTION ImportEZProxyLogOperation;

                END TRY
                BEGIN CATCH

                    ROLLBACK TRANSACTION ImportEZProxyLogOperation;

                    THROW;

                END CATCH";

            try
            {
                Int32 rowsModified = ExecuteCommand(updateTableQuery);

                HandleUnmodifiedDatabase(rowsModified, new[] { tempEZProxyTableName });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException);
                    Console.WriteLine(ex.InnerException.Message);
                }
                throw;
            }
        }

        public void BulkImportVendorTransactions(IDataReader journalDataReader, IDataReader databases)
        {
            String tempTableName = $"[Journals].[TransactionsImport-{Guid.NewGuid()}]";
            String tempTableName2 = $"[Journals].[DatabaseImport-{Guid.NewGuid()}]";

            BulkCopy(
                @"CREATE TABLE {0}(
                    [Vendor] [nvarchar](MAX) NOT NULL,
                    [Database] [nvarchar](MAX) NOT NULL,
	                [Name] [nvarchar](MAX) NOT NULL,
	                [PrintISSN] [nvarchar](16) NULL,
	                [OnlineISSN] [nvarchar](16) NULL,
	                [FullText] [int] NOT NULL,
                    [RunDate] [date] NOT NULL
                ) ON [PRIMARY];", tempTableName, journalDataReader);

            BulkCopy(
                @"CREATE TABLE {1}(
                    [Vendor] [nvarchar](MAX) NOT NULL,
                    [Database] [nvarchar](MAX) NOT NULL,
	                [Searches] [int] NULL,
	                [Result_Clicks] [int] NULL,
                    [Record_Views] [int] NULL,
                    [TimeStamp] [date] NOT NULL
                ) ON [PRIMARY];", tempTableName2, databases);

            String updateTableQuery = $@"
                BEGIN TRANSACTION ImportVendorTransactions

                BEGIN TRY
                    USE [Statistics];
                    INSERT INTO [Journals].[Journals] (JournalName,PrintISSN,OnlineISSN)
                    SELECT innerTable.Name,innerTable.PrintISSN,innerTable.OnlineISSN
                    FROM (
                        SELECT A.Name,A.PrintISSN,A.OnlineISSN
	                    FROM {tempTableName} AS A
	                    ) AS innerTable
                    LEFT JOIN [Journals].[Journals] AS B
                    ON (innerTable.PrintISSN IS NOT NULL AND innerTable.PrintISSN = B.PrintISSN) OR
                       (innerTable.PrintISSN IS NULL AND B.PrintISSN IS NULL AND innerTable.OnlineISSN IS NOT NULL AND innerTable.OnlineISSN = B.OnlineISSN) OR
                       (innerTable.PrintISSN IS NULL AND innerTable.OnlineISSN IS NULL AND B.PrintISSN IS NULL AND B.OnlineISSN IS NULL AND innerTable.Name = B.JournalName) 
                    WHERE B.JournalID IS NULL;

                    INSERT INTO [Journals].[Databases] (DatabaseName)
                    SELECT innerTable.databaseName
                    FROM (
	                    SELECT A.[Database] AS databaseName
	                    FROM {tempTableName} AS A
                        GROUP BY A.[Database]
	                    HAVING A.[Database] NOT IN (
							SELECT B.DatabaseName FROM [Journals].[Databases] AS B
                            )
	                    ) AS innerTable;

                    INSERT INTO [Journals].[Vendors] (VendorName)
                    SELECT innerTable.VendorName
                    FROM (
	                    SELECT A.Vendor AS VendorName
	                    FROM {tempTableName} AS A
	                    WHERE A.Vendor NOT IN (
				            SELECT B.VendorName FROM [Journals].[Vendors] AS B
				            )
	                    GROUP BY A.Vendor
	                    ) AS innerTable;

                    MERGE [Journals].[JournalRecords] AS R
                    USING (
	                    SELECT VendorId,DatabaseID,JournalID,RunDate,FullText AS [FullText]
	                    FROM {tempTableName} A
	                    INNER JOIN [Journals].[Vendors] V
	                    ON A.Vendor = V.VendorName
	                    INNER JOIN [Journals].[Databases] D
	                    ON A.[Database] = D.DatabaseName
	                    INNER JOIN [Journals].[Journals] J
	                   ON (A.PrintISSN IS NOT NULL AND A.PrintISSN = J.PrintISSN) OR
						(A.PrintISSN IS NULL AND J.PrintISSN IS NULL AND A.OnlineISSN IS NOT NULL AND A.OnlineISSN = J.OnlineISSN) OR
						(A.PrintISSN IS NULL AND A.OnlineISSN IS NULL AND J.PrintISSN IS NULL AND J.OnlineISSN IS NULL AND A.Name = J.JournalName) 
	                    ) AS S
                    ON (S.VendorId = R.VendorID AND S.DatabaseID = R.DatabaseID AND S.JournalID = R.JournalID AND S.RunDate = R.RunDate)
                    WHEN NOT MATCHED THEN
	                    INSERT (VendorID,DatabaseID,JournalID,RunDate,FullText) VALUES (S.VendorId,S.DatabaseID,S.JournalID,S.RunDate,S.FullText)
                    WHEN MATCHED THEN
	                    UPDATE SET R.FullText = S.FullText,
	                    R.ModifiedDate = SYSDATETIME();
                    
                    
                    
                    INSERT INTO	[Journals].[Databases] (DatabaseName)
	                SELECT DR.[Database] AS databaseName
	                FROM {tempTableName2} AS DR
	                WHERE DR.[Database] NOT IN (
							                SELECT D.DatabaseName FROM [Journals].[Databases] AS D
							                );

                    INSERT INTO	[Journals].[Vendors] (VendorName)
	                SELECT A.Vendor AS VendorName
	                FROM {tempTableName2} AS A
                    GROUP BY A.Vendor
	                HAVING A.Vendor NOT IN (
				                SELECT B.VendorName FROM [Journals].[Vendors] AS B
				                );

                    MERGE [Journals].[DatabaseRecords] AS T
                    USING (
	                    SELECT V.VendorID,D.DatabaseID,TimeStamp,A.Searches,A.Result_Clicks,A.Record_Views,SUM(J.FullText) AS [FullText]
	                    FROM {tempTableName2} A
	                    INNER JOIN [Journals].[Vendors] V
	                    ON A.Vendor = V.VendorName
	                    INNER JOIN [Journals].[Databases] D
	                    ON A.[Database] = D.DatabaseName
	                    INNER JOIN [Journals].[JournalRecords] J
	                    ON (J.DatabaseID = D.DatabaseID)
                        GROUP BY V.VendorId,D.DatabaseID,TimeStamp,A.Searches,A.Result_Clicks,A.Record_Views
                    ) AS S
                    ON (S.VendorID = T.VendorID AND S.DatabaseID = T.DatabaseID AND S.[TimeStamp] = T.RunDate)
                    WHEN NOT MATCHED THEN
	                    INSERT (VendorID,DatabaseID,RunDate,Searches,ResultClicks,RecordViews,FullText) VALUES (S.VendorID,S.DatabaseID,S.[TimeStamp],S.Searches,S.Result_Clicks,S.Record_Views,FullText)
                    WHEN MATCHED THEN
	                    UPDATE SET T.ResultClicks = S.Result_Clicks,
	                    T.Searches = S.Searches,
	                    T.RecordViews = S.Record_Views,
	                    T.ModifiedDate = SYSDATETIME(),
	                    T.FullText = S.FullText;

                    MERGE [Journals].[VendorRecords] AS T
                    USING (
	                    SELECT DR.VendorID,RunDate,SUM(DR.Searches) AS Searches,Sum(DR.ResultClicks) AS ResultClicks,SUM(DR.RecordViews) AS RecordViews,SUM(DR.[FullText]) AS [FullText]
	                    FROM [Journals].[DatabaseRecords] AS DR
	                    GROUP BY DR.VendorID,DR.RunDate
	                    ) AS S
                    ON (S.VendorID = T.VendorID AND S.RunDate = T.RunDate)
                    WHEN NOT MATCHED THEN
	                    INSERT (VendorID,RunDate,Searches,ResultClicks,RecordViews,FullText) VALUES (S.VendorID,S.RunDate,S.Searches,S.ResultClicks,S.RecordViews,S.[FullText])
                    WHEN MATCHED THEN
	                    UPDATE SET T.Searches = S.Searches,
	                    T.ResultClicks = S.ResultClicks,
	                    T.RecordViews = S.RecordViews,
	                    T.[FullText] = S.[FullText],
	                    T.ModifiedDate = SYSDATETIME();
                    
                    DROP TABLE {tempTableName};
                    DROP TABLE {tempTableName2};

                    COMMIT TRANSACTION ImportVendorTransactions;
    
                END TRY
                BEGIN CATCH

                    ROLLBACK TRANSACTION ImportVendorTransactions;

                    THROW;
                    
                END CATCH";

            try
            {
                Int32 rowsModified = ExecuteCommand(updateTableQuery);

                HandleUnmodifiedDatabase(rowsModified, new[] { tempTableName, tempTableName2 });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException);
                    Console.WriteLine(ex.InnerException.Message);
                }
                throw;
            }
        }

        private void DeleteTemporaryTables(params string[] tables)
        {
            if (tables.Length < 1) return;

            string dropCommand = tables.Aggregate("", (current, table) => current + "DROP TABLE " + table + "; ");

            try
            {
                ExecuteCommand(dropCommand);
            }
            catch
            {
                // ignored
            }
        }
    }
}