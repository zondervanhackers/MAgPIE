using System;
using System.Diagnostics.Contracts;
using ZondervanLibrary.SharedLibrary.Factory;
using ZondervanLibrary.SharedLibrary.Repository;

namespace ZondervanLibrary.Harvester.Core.Repository.Database
{
    public class SqlServerDatabaseRepository<TDataContext> : RepositoryBase, IDatabaseRepository<TDataContext>
        where TDataContext : IDataContext
    {
        private readonly TDataContext dataContext;
        private readonly SqlServerDatabaseRepositoryArguments arguments;

        public SqlServerDatabaseRepository(SqlServerDatabaseRepositoryArguments arguments, IFactory<TDataContext, String> dataContextFactory)
        {
            Contract.Requires(arguments != null);
            Contract.Requires(arguments != null);
            Contract.Requires(arguments.Database != null);
            Contract.Requires(arguments.Username != null);
            Contract.Requires(arguments.Password != null);
            Contract.Requires(dataContextFactory != null);

            Name = arguments.Name;
            RepositoryId = new Guid();


            String connectionString = arguments.Authentication == SqlServerAuthenticationMethod.Windows ? 
                String.Format("Data Source={0};Initial Catalog={1};Integrated Security=True;MultipleActiveResultSets=true;Max Pool Size=200", arguments.Server, arguments.Database) :
                String.Format("Data Source={0};Integrated Security=True;Initial Catalog={1};User ID={2};Password={3};MultipleActiveResultSets=true;Max Pool Size=200", arguments.Server, arguments.Database, arguments.Username, arguments.Password);

            dataContext = dataContextFactory.CreateInstance(connectionString);
            this.arguments = arguments;
        }

        public TDataContext DataContext => dataContext;

        public override void Dispose()
        {
            if (dataContext != null)
                dataContext.Dispose();
        }

        public override string ConnectionString => $"SQL Server | Server = {arguments.Server}, Database = {arguments.Database}, {(arguments.Username == null ? "Integrated Security" : "Username = " + arguments.Username)}";
    }
}
