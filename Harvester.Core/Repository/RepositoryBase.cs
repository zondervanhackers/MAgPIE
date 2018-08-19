using System;

namespace ZondervanLibrary.Harvester.Core.Repository
{
    public abstract class RepositoryBase : IRepository
    {
        public Guid RepositoryId { get; set; }

        public string Name { get; set; }

        public abstract string ConnectionString { get; }

        public abstract void Dispose();
    }
}
