using System.Windows;
using MahApps.Metro.Controls;
using SimpleInjector;
using ZondervanLibrary.Harvester.Wpf.Dialogs.Repository.ViewModels;

namespace ZondervanLibrary.Harvester.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Container container = DependencyInjectionBindings.CreateBindings(this);

            DataContext = container.GetInstance<MainViewModel>();
        }
    }
}
