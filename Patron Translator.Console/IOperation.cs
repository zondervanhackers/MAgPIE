using System;

namespace ZondervanLibrary.PatronTranslator.Console
{
    public interface IOperation
    {
        /// <summary>
        /// Executes the operation.
        /// </summary>
        /// <param name="runDate">The date at which the operation is being run (some operations require knowledge of temporality).</param>
        /// <param name="reportProgress">
        ///     A function to report the current progress of the operation.  
        ///     The first paramenter is the percentage complete from 0.0 to 1.0.
        ///     The second is an optional message to be added to the message stack.
        /// </param>
        void Execute(DateTime runDate, Action<Double, String> reportProgress);
    }
}
