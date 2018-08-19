using System;

namespace ZondervanLibrary.Harvester.Wpf.Windows
{
    /// <summary>
    /// Provides an interface that exposes the <see cref="System.Windows.Window.ShowDialog()"/> method.
    /// </summary>
    public interface IWindow
    {
        /// <inheritdoc cref="System.Windows.Window.ShowDialog()"/>
        Boolean? ShowDialog();
    }
}
