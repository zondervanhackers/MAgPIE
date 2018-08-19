using System;
using System.Collections.Generic;
using System.Windows;

using SimpleInjector;
using ZondervanLibrary.SharedLibrary.Factory;
using ZondervanLibrary.Harvester.Wpf.Windows;
using ZondervanLibrary.Harvester.Wpf.Dialogs.Repository.ViewModels;
using ZondervanLibrary.Harvester.Wpf.Dialogs.Repository.Add;

namespace ZondervanLibrary.Harvester.Wpf
{
    public static class DependencyInjectionBindings
    {
        public static Container CreateBindings(Window window)
        {
            Container container = new Container();

            // The constructors for the Window classes require a reference to the parent
            // window, which the calling view model should not be aware of.  Therefore
            // we pass it in here.
            container.Register(() => window);

            container.Register<IFactory<IAddRepositoryDialog>, EmitFactory0<IAddRepositoryDialog, AddRepositoryDialog, IFactory<IAddRepositoryDialogViewModel>, IFactory<IWindow, IAddRepositoryDialogViewModel>>>(Lifestyle.Singleton);
            container.Register<IFactory<IAddRepositoryDialogViewModel>, EmitFactory0<IAddRepositoryDialogViewModel, AddRepositoryDialogViewModel, IEnumerable<IRepositoryGroupViewModel>>>(Lifestyle.Singleton);
            container.Register<IFactory<IWindow, IAddRepositoryDialogViewModel>, EmitFactory1<IWindow, AddRepositoryDialogWindow, IAddRepositoryDialogViewModel, Window>>(Lifestyle.Singleton);

            container.RegisterAll<IRepositoryGroupViewModel>(typeof(DirectoryRepositoryGroupViewModel), typeof(DatabaseRepositoryGroupViewModel), typeof(JournalRepositoryGroupViewModel));

            container.RegisterAll<IDirectoryRepositoryViewModel>(typeof(FolderDirectoryRepositoryViewModel), typeof(FtpDirectoryRepositoryViewModel), typeof(SftpDirectoryRepositoryViewModel));

            container.RegisterAll<IDatabaseRepositoryViewModel>(typeof(SqlServerDatabaseRepositoryViewModel));

            container.RegisterAll<IJournalRepositoryViewModel>(typeof(SushiJournalRepositoryViewModel));

#if DEBUG
            try
            {
                container.Verify();
            }
            catch (Exception ex)
            {
                Console.WriteLine("asdf");
            }
#endif

            return container;
        }
    }
}
