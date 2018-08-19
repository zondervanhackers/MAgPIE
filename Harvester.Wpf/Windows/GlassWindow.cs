using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace ZondervanLibrary.Harvester.Wpf.Windows
{
    /// <summary>
    /// GlassWindows adds properties to the Window class to extend the Aero glass into the client area.
    /// </summary>
    public class GlassWindow : Window
    {
        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern bool DwmIsCompositionEnabled();

        /// <summary>
        /// The dependency property backing <see cref="GlassThickness"/>.
        /// </summary>
        public static readonly DependencyProperty GlassThicknessProperty = DependencyProperty.Register("GlassThickness", typeof(Thickness), typeof(GlassWindow), new PropertyMetadata(new Thickness(0, 0, 0, 0), GlassThicknessChanged));

        /// <summary>
        /// Gets or sets the thickness of the glass for each edge of the client window.
        /// </summary>
        public Thickness GlassThickness
        {
            get => (Thickness)GetValue(GlassThicknessProperty);
            set => SetValue(GlassThicknessProperty, value);
        }

        private static void GlassThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GlassWindow window = (GlassWindow)d;

            if (window.GlassAvailable && window.GlassEnabled)
            {
                window.UpdateGlassState(window.GlassThickness);
            }
        }

        /// <summary>
        /// The dependency property backing for <see cref="GlassAvailable"/>.
        /// </summary>
        public static readonly DependencyProperty GlassAvailableProperty = DependencyProperty.Register("GlassAvailable", typeof(Boolean), typeof(GlassWindow), new PropertyMetadata(IsDwmAvailable(), GlassAvailableChanged));

        /// <summary>
        /// Gets the availablity of Windows Aero glass effects.
        /// </summary>
        /// <remarks>
        ///     <para>This property is determined by the value of IsDwmAvailable() (dwmapi.dll).</para>
        /// </remarks>
        public Boolean GlassAvailable => (Boolean)GetValue(GlassAvailableProperty);

        private static void GlassAvailableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GlassWindow window = (GlassWindow)d;

            if (window.GlassEnabled && window.GlassAvailable)
            {
                window.UpdateGlassState(window.GlassThickness);
            }
        }

        /// <summary>
        /// The dependecy property backing for <see cref="GlassEnabled"/>.
        /// </summary>
        public static readonly DependencyProperty GlassEnabledProperty = DependencyProperty.Register("GlassEnabled", typeof(Boolean), typeof(GlassWindow), new PropertyMetadata(false, GlassEnabledChanged));

        /// <summary>
        /// Gets or sets whether or not Aero Glass is enabled for this window.
        /// </summary>
        public Boolean GlassEnabled
        {
            get => (Boolean)GetValue(GlassEnabledProperty);
            set => SetValue(GlassEnabledProperty, value);
        }

        private static void GlassEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GlassWindow window = (GlassWindow)d;

            if (window.GlassAvailable)
            {
                window.UpdateGlassState(window.GlassEnabled
                    ? window.GlassThickness
                    : new Thickness {Bottom = 0, Left = 0, Right = 0, Top = 0});
            }
        }

        /// <summary>
        /// Hooks into the windows message pump to observe changes in IsDwmAvailable()
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);

            SetValue(GlassAvailableProperty, IsDwmAvailable());

            if (GlassAvailable && GlassEnabled)
            {
                UpdateGlassState(GlassThickness);
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // WM_DWMCOMPOSITIONCHANGED
            if (message == 0x031E)
            {
                Boolean available = IsDwmAvailable();

                SetValue(GlassAvailableProperty, available);
            }

            return IntPtr.Zero;
        }

        private static Boolean IsDwmAvailable()
        {
            try
            {
                return Environment.OSVersion.Version.Major >= 6 && DwmIsCompositionEnabled();
            }
            catch (DllNotFoundException)
            {
                return false;
            }
        }

        private void UpdateGlassState(Thickness thickness)
        {
            if (GlassEnabled == false)
                return;

            if (IsInitialized == false)
                return;

            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            if (hwnd == IntPtr.Zero)
                return;

            Background = Brushes.Transparent;
            HwndSource.FromHwnd(hwnd).CompositionTarget.BackgroundColor = Colors.Transparent;

            MARGINS margins = new MARGINS(thickness);
            DwmExtendFrameIntoClientArea(hwnd, ref margins);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct MARGINS
    {
        public MARGINS(Thickness t)
        {
            Left = (int)t.Left;
            Right = (int)t.Right;
            Top = (int)t.Top;
            Bottom = (int)t.Bottom;
        }

        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
    }
}
