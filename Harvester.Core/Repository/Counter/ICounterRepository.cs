using System;
using System.Collections.Generic;

namespace ZondervanLibrary.Harvester.Core.Repository.Counter
{
    /// <summary>
    /// Provides an interface for repositories that provide journal/database/vendor statistics.
    /// </summary>
    public interface ICounterRepository : IRepository
    {
        IEnumerable<CounterReport> AvailableReports { get; }

        IEnumerable<CounterRecord> RequestRecords(DateTime runDate, CounterReport report);

        Action<string> LogMessage { set; }
    }
}
