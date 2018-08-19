using System;
using System.Collections;
using System.Collections.Generic;

namespace ZondervanLibrary.Harvester.Core.Scheduling
{
    public class IntervalSchedule : ISchedule
    {
        private readonly DateTime startDate;
        private readonly DateTime? endDate;
        private readonly Func<DateTime, DateTime> advanceRunDate;

        public IntervalSchedule(IntervalScheduleArguments arguments) : this(arguments.StartDate, arguments.EndDate, arguments.Unit, arguments.Interval) {}

        private IntervalSchedule(DateTime startDate, DateTime? endDate, IntervalUnit intervalUnit, double interval)
        {
            this.startDate = startDate;
            this.endDate = endDate;

            double increment;

            switch (intervalUnit)
            {
                case IntervalUnit.Seconds:
                    increment = interval;
                    advanceRunDate = d => d.AddSeconds(increment);
                    break;
                case IntervalUnit.Minutes:
                    increment = interval;
                    advanceRunDate = d => d.AddMinutes(increment);
                    break;
                case IntervalUnit.Hours:
                    increment = interval;
                    advanceRunDate = d => d.AddHours(increment);
                    break;
                case IntervalUnit.Days:
                    increment = interval;
                    advanceRunDate = d => d.AddDays(increment);
                    break;
                case IntervalUnit.Weeks:
                    increment = 7.0 * interval;
                    advanceRunDate = d => d.AddDays(increment);
                    break;
                case IntervalUnit.Months:
                    if (interval % 1 > 0)
                        throw new Exceptions.ConfigurationFileException($"Cannot schedule Operation for {interval} Months later");
                    else
                    {
                        increment = interval;
                        advanceRunDate = d => d.AddMonths((int)increment);
                    }
                    break;
                case IntervalUnit.Quarters:
                    if (interval % 1 > 0)
                        throw new Exceptions.ConfigurationFileException($"Cannot schedule Operation for {interval} Quarters later");
                    else
                    {
                        increment = interval * 3;
                        advanceRunDate = d => d.AddMonths((int)increment);
                    }
                    break;
                case IntervalUnit.Years:
                    if (interval % 1 > 0)
                        throw new Exceptions.ConfigurationFileException($"Cannot schedule Operation for {interval} Years later");
                    else
                    {
                        increment = interval;
                        advanceRunDate = d => d.AddYears((int)increment);
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public IEnumerator<DateTime> GetEnumerator()
        {
            DateTime runDate = startDate;
            yield return runDate;

            while (endDate == null || runDate < endDate)
            {
                runDate = advanceRunDate(runDate);
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
