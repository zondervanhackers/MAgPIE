using System;
using System.Diagnostics.Contracts;
using System.ServiceModel;
using ZondervanLibrary.Harvester.Communication;
using ZondervanLibrary.Harvester.Wpf.Communication;
using ZondervanLibrary.Harvester.Wpf.Dialogs.Repository.Add;
using ZondervanLibrary.SharedLibrary;
using ZondervanLibrary.SharedLibrary.Collections;
using ZondervanLibrary.SharedLibrary.Factory;
using OperationContext = ZondervanLibrary.Harvester.Core.Operations.OperationContext;

namespace ZondervanLibrary.Harvester.Wpf
{
    public class MainViewModel : ViewModelBase
    {
#if Production
        static string Configuration = "Production";
#elif Test
        static string Configuration = "Test";
#elif Debug
        static string Configuration = "Debug";
#else
        static string Configuration = "";
#endif

        private readonly IFactory<IAddRepositoryDialog> _repositoryProviderFactory;
        private IHarvesterServiceConnection _serviceConnection;

        public MainViewModel(IFactory<IAddRepositoryDialog> repositoryProviderFactory)
        {
            Contract.Requires(repositoryProviderFactory != null);

            _repositoryProviderFactory = repositoryProviderFactory;

            HarvesterClientConnection connection = new HarvesterClientConnection();
            InstanceContext context = new InstanceContext(connection);
            NetNamedPipeBinding binding = new NetNamedPipeBinding();

            DuplexChannelFactory<IHarvesterServiceConnection> channelFactory = new DuplexChannelFactory<IHarvesterServiceConnection>(
                context,
                binding,
                new EndpointAddress($"net.pipe://localhost/VAMP_{Configuration}/IHarvesterServiceConnection"));

            _serviceConnection = channelFactory.CreateChannel();

            _serviceConnection.Subscribe();

            _items = new SlaveObservableCollection<OperationContext>(() => _serviceConnection.RunningOperations());

            connection.OnRunningOperationsCollectionChangedCallback = args => _items.OnMasterCollectionChanged(args);
        }

        private readonly SlaveObservableCollection<OperationContext> _items;
        public SlaveObservableCollection<OperationContext> Items => _items;

        public void CreateRepositoryProvider()
        {
            //Renci.SshNet.KeyboardInteractiveAuthenticationMethod keyboardAuthentication = new Renci.SshNet.KeyboardInteractiveAuthenticationMethod("[Username]");
            //Renci.SshNet.PasswordAuthenticationMethod passwordAuthentication = new Renci.SshNet.PasswordAuthenticationMethod("[Username]", "[Password]");

            //keyboardAuthentication.AuthenticationPrompt += (object sender, Renci.SshNet.Common.AuthenticationPromptEventArgs e) =>
            //{
            //    foreach (var prompt in e.Prompts)
            //    {
            //        if (prompt.Request.IndexOf("Password:", StringComparison.InvariantCultureIgnoreCase) != -1)
            //        {
            //            prompt.Response = "[Password]";
            //        }
            //    }
            //};

            //Renci.SshNet.ConnectionInfo connectionInfo = new Renci.SshNet.ConnectionInfo("ftp2.oclc.org", "[Username]", passwordAuthentication, keyboardAuthentication);

            //Renci.SshNet.SftpClient client = new Renci.SshNet.SftpClient(connectionInfo);

            //client.Connect();



            //IAddRepositoryDialog dialog = _repositoryProviderFactory.CreateInstance();

            //if (dialog.ShowDialog() == true)
            //{
            //    // Add new thingy to service
            //}



            //var list = new List<Int32>(){1, 2, 3};

            //var x1 = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

            //var x21 = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, list);



            //_items.AddRange(new List<String>() { "a", "b" });

            //_items.AddRange(new List<String>() { "aa", "bb", "cc" });

            //System.Windows.MessageBox.Show(_notificationSubscription.Subscribe());

            //InstanceContext context = new InstanceContext(new HarvesterClientConnection());
            //NetNamedPipeBinding binding = new NetNamedPipeBinding();

            //DuplexChannelFactory<IHarvesterServiceConnection> httpFactory = new DuplexChannelFactory<IHarvesterServiceConnection>(
            //    context,
            //    binding,
            //    new EndpointAddress("net.pipe://localhost/harvest"));

            //httpFactory.Closed += (object sender, EventArgs args) =>
            //{
            //    Debug.WriteLine("Connection Closed.");
            //};

            //httpFactory.Faulted += (object sender, EventArgs args) =>
            //{
            //    Debug.WriteLine("Connection Faulted.");
            //};

            //httpFactory.Opened += (object sender, EventArgs args) =>
            //{
            //    Debug.WriteLine("Connection Opened.");
            //};

            //_serviceConnection = httpFactory.CreateChannel();

            //_serviceConnection.Subscribe();

            ////System.Threading.Thread.Sleep(20000);

            ////Debug.WriteLine("Running RunningOperations.");

            foreach (var num in _serviceConnection.RunningOperations())
            {
                System.Windows.MessageBox.Show(num.ToString());
            }

            //DateTime[] times = new DateTime[] { 
            //    new DateTime(2015, 5, 3)
            //};

            //foreach (DateTime date in times)
            //{
            //    IDirectoryProvider directoryProvider = new FileDirectoryProvider(@"F:\Zondervan Library\Test Data");
            //    IStreamProviderFactory streamProviderFactory = new StreamProviderFactory(directoryProvider, "ITU.Item_inventories.{0:yyyyMMdd}.txt");

            //    IStatisticsDataContext dataContext = new StatisticsDataContext(@"Data Source=MICAH-PC\SQLEXPRESS;Initial Catalog=Statistics;Integrated Security=True");
            //    IRepositoryFactory<IStatisticsDataContext> repositoryFactory = new LinqToSqlRepositoryFactory<IStatisticsDataContext>(dataContext);

            //    IDatabaseProviderFactory<IStatisticsDataContext> databaseProviderFactory = new LinqToSqlDatabaseProviderFactory<IStatisticsDataContext>(dataContext, repositoryFactory);
            //    IStreamParser<WmsInventoryRecord> streamParser = new DelimitedStreamParser<WmsInventoryRecord>();

            //    IOperation operation = new ImportWmsInventoryOperation(streamProviderFactory, databaseProviderFactory, streamParser);

            //    operation.Execute(date);
            //}

            //System.Windows.MessageBox.Show("Done.");

            //DateTime[] times = new DateTime[] {
            //    new DateTime(2014, 6, 8),
            //    new DateTime(2014, 6, 9),
            //    new DateTime(2014, 6, 10),
            //    new DateTime(2014, 6, 11),
            //    new DateTime(2014, 6, 12),
            //    new DateTime(2014, 6, 13),
            //    new DateTime(2014, 6, 14),
            //    new DateTime(2014, 6, 15),
            //    new DateTime(2014, 6, 16),
            //    new DateTime(2014, 6, 17)
            //};

            //System.Diagnostics.Stopwatch stopwatch = new Stopwatch();

            //stopwatch.Start();

            //foreach (DateTime time in times)
            //{
            //    IDirectoryProvider directoryProvider = new FileDirectoryProvider(@"F:\Zondervan Library\Test Data");
            //    IStreamProviderFactory streamProviderFactory = new StreamProviderFactory(directoryProvider, "ITU.ITUZ.All_checked_out_items.{0:yyyyMMdd}.txt");

            //    IStatisticsDataContext dataContext = new StatisticsDataContext(@"Data Source=.\SQLEXPRESS;Initial Catalog=Statistics;Integrated Security=True");
            //    IRepositoryFactory<IStatisticsDataContext> repositoryFactory = new LinqToSqlRepositoryFactory<IStatisticsDataContext>(dataContext);

            //    IDatabaseProviderFactory<IStatisticsDataContext> databaseProviderFactory = new LinqToSqlDatabaseProviderFactory<IStatisticsDataContext>(dataContext, repositoryFactory);
            //    IStreamParser<WmsTransactionRecord> streamParser = new DelimitedStreamParser<WmsTransactionRecord>();

            //    IOperation operation = new ImportWmsTransactionsOperation(streamProviderFactory, databaseProviderFactory, streamParser);

            //    operation.Execute(time);
            //}

            //stopwatch.Stop();

            //System.Windows.MessageBox.Show(String.Format("Elapsed: {0}ms", stopwatch.ElapsedMilliseconds));
        }
    }
}
