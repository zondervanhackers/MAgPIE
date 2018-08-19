using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ZondervanLibrary.Harvester.Core.Operations
{
    /// <summary>
    /// A class that contains an operation along with the time it should be run at.
    /// </summary>
    /// <remarks>
    ///     <para>This class is immutable.</para>
    /// </remarks>
    public class OperationContext : IComparable<OperationContext>
    {
        private readonly IEnumerator<DateTime> _nextSchedule;
        private readonly Boolean _nextExists;

        public OperationContext(IOperation operation, IEnumerator<DateTime> schedule)
        {
            Contract.Requires(operation != null);
            Contract.Requires(schedule != null);

            Operation = operation;
            RunDate = schedule.Current;

            _nextSchedule = schedule;
            _nextExists = _nextSchedule.MoveNext();

            CurrentRetry = 0;
        }

        public DateTime RunDate { get; }

        public IOperation Operation { get; }

        public int MaximumRunsPerDay { get; set; }

        public int MaximumConcurrentlyRunning { get; set; }

        public int CurrentRetry { get; private set; }

        public OperationContext GetNext()
        {
            return _nextExists ? new OperationContext(Operation, _nextSchedule) { MaximumRunsPerDay = MaximumRunsPerDay, MaximumConcurrentlyRunning = MaximumConcurrentlyRunning } : null;
        }

        public OperationContext GetRetry()
        {
            CurrentRetry++;
            return this;
        }

        /// <inheritdoc/>
        public int CompareTo(OperationContext other)
        {
            return RunDate.CompareTo(other.RunDate);
        }
    }
}
