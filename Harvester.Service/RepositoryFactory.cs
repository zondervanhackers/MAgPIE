using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;

using SimpleInjector;

using ZondervanLibrary.SharedLibrary.Factory;

using ZondervanLibrary.Harvester.Core.Repository;
using ZondervanLibrary.Harvester.Core.Repository.Counter;
using ZondervanLibrary.Harvester.Core.Repository.Database;
using ZondervanLibrary.Harvester.Core.Repository.Directory;

using ZondervanLibrary.Statistics.Entities;

namespace ZondervanLibrary.Harvester.Service
{
    public class RepositoryFactory
    {
        //public ICounterRepository CreateCounterRepository(RepositoryArgumentsBase arguments)
        //{
        //    if (arguments.GetType() == typeof(SushiCounterRepositoryArguments))
        //    {

        //    }
            
        //}

        public IDatabaseRepository<IStatisticsDataContext> CreateDatabaseRepository(RepositoryArgumentsBase arguments)
        {
            if (arguments.GetType() == typeof(SqlServerDatabaseRepositoryArguments))
            {
                var factory = SimpleInjector.Container.GetInstance<IFactory<IStatisticsDataContext, String>>();
                return new SqlServerDatabaseRepository<IStatisticsDataContext>(arguments as SqlServerDatabaseRepositoryArguments, factory);
            }

            throw new NotImplementedException();
        }
    }
}
