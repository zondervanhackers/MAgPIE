using System;

namespace ZondervanLibrary.Harvester.Wpf.Dialogs
{
    /// <summary>
    /// Provides a dialog that communications with the user.
    /// </summary>
    /// <remarks>
    ///     <para>When implemented in a derived class, <see cref="IDialog"/> provides an interface to "dialog" with the user by either displaying information or prompting the user for input.</para>
    /// </remarks>
    public interface IDialog
    {
        /// <summary>
        /// Shows the dialog to the user and returns the result of the communication.
        /// </summary>
        /// <returns>
        ///     <para>
        ///         <list type="table">
        ///             <listheader>
        ///                 <term>Result</term>
        ///                 <term>Condition</term>
        ///             </listheader>
        ///             <item>
        ///                 <term><see langword="true" /></term>
        ///                 <description>The dialog completed successfully.</description>
        ///             </item>
        ///             <item>
        ///                 <term><see langword="false" /></term>
        ///                 <description>The user cancelled the dialog.</description>
        ///             </item>
        ///         </list>
        ///     </para>
        /// </returns>
        Boolean ShowDialog();
    }
}
