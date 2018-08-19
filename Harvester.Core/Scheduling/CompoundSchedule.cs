using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;

using ZondervanLibrary.SharedLibrary.Collections;

namespace ZondervanLibrary.Harvester.Core.Scheduling
{
    /// <summary>
    /// Represents a Schedule that compounds multiple child schedules into one.
    /// </summary>
    public class CompoundSchedule : ISchedule
    {
        private readonly IEnumerable<ISchedule> _schedules;

        public CompoundSchedule(IEnumerable<ISchedule> schedules)
        {
            Contract.Assert(schedules != null);

            _schedules = schedules;
        }

        /// <inheritdoc/>
        public IEnumerator<DateTime> GetEnumerator()
        {
            HeapPriorityQueue<ComparableEnumerator<DateTime>> queue = new HeapPriorityQueue<ComparableEnumerator<DateTime>>(_schedules.Select(s => 
                {
                    IEnumerator<DateTime> e = s.GetEnumerator();
                    return new Tuple<Boolean, IEnumerator<DateTime>>(e.MoveNext(), e);
                }).Where(t => t.Item1)
                  .Select(t => new ComparableEnumerator<DateTime>(t.Item2)));

            while (queue.Count > 0)
            {
                ComparableEnumerator<DateTime> schedule = queue.Dequeue();
                DateTime runDate = schedule.Enumerator.Current;

                if (schedule.Enumerator.MoveNext())
                {
                    queue.Enqueue(schedule);
                }

                yield return runDate;
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
