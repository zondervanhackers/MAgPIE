using System;
using System.Collections.Generic;

namespace ZondervanLibrary.Harvester.Core.Scheduling
{
    /// <summary>
    /// Represents a schedule as an ordered list of dates.
    /// </summary>
    public interface ISchedule : IEnumerable<DateTime>
    {

    }
}
