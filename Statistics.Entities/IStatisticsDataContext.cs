using System;
using System.Data;
using ZondervanLibrary.SharedLibrary.Repository;

namespace ZondervanLibrary.Statistics.Entities
{
    public interface IStatisticsDataContext : IDataContext
    {
        Action<String> LogMessage { get; set; }
        void BulkCopy(String createTableSql, String tableName, IDataReader dataReader, int timeout = 0);

        void BulkImportCounterTransactions(IDataReader recordsDataReader, IDataReader identifierDataReader, IDataReader metricDataReader);

        void BulkImportStatista(IDataReader statistaDataReader);

        void BulkImportDemographics(IDataReader patronDataReader, IDataReader majorDataReader, IDataReader residenceDataReader);

        void BulkImportInventory(IDataReader dataReader);

        void BulkImportTransactions(IDataReader dataReader);

        void BulkImportVendorTransactions(IDataReader journals, IDataReader databases);        

        void BulkImportEZProxyAudit(IDataReader EZProxyDataReader);

        void BulkImportEZProxyLog(IDataReader EZProxyDataReader);
    }
}
