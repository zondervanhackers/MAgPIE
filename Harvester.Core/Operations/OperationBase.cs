using System;
using System.Collections.Generic;
using System.Linq;
using ZondervanLibrary.Harvester.Core.Repository;
using ZondervanLibrary.Harvester.Core.Repository.Database;
using ZondervanLibrary.Harvester.Core.Repository.Directory;
using ZondervanLibrary.Harvester.Entities;

namespace ZondervanLibrary.Harvester.Core.Operations
{
    public abstract class OperationBase : IOperation
    {
        public string Name { get; set; }

        public int OperationID { get; set; }

        public abstract void Execute(DateTime runDate, Action<String> logMessage, System.Threading.CancellationToken cancellationToken);

        protected void UpdateHarvesterRecord(Action<string> logMessage, IEnumerable<DirectoryObjectMetadata> sourceFiles, string sourceName, RepositoryArgumentsBase harvesterArgs)
        {
            using (IDatabaseRepository<IHarvesterDataContext> harvester = RepositoryFactory.CreateHarvesterRepository(harvesterArgs))
            {
                logMessage($"Connected to database '{harvester.Name}' ({harvester.ConnectionString})");

                int insertedRecords = 0;

                Entities.Repository repository = harvester.DataContext.Repositories.First(y => y.Name == sourceName);

                if (OperationID == 0)
                {
                    logMessage("Warning: OperationID was not set properly. Correcting this.");
                    OperationID = harvester.DataContext.Operations.First(d => d.Name == Name).ID;
                }

                Dictionary<String, DirectoryRecord> dictionary = harvester.DataContext.DirectoryRecords.Where(d => d.OperationID == OperationID && d.RepositoryID == repository.ID).ToDictionary(d => d.FilePath);

                foreach (DirectoryObjectMetadata file in sourceFiles)
                {
                    if (!dictionary.ContainsKey(file.Path))
                    {
                        harvester.DataContext.DirectoryRecords.InsertOnSubmit(new DirectoryRecord
                        {
                            OperationID = OperationID,
                            RepositoryID = repository.ID,
                            FilePath = file.Path,
                            FileModifiedDate = file.ModifiedDate,
                            CreationDate = DateTime.Now,
                            ModifiedDate = DateTime.Now
                        });
                        insertedRecords++;
                    }
                    else
                    {
                        DirectoryRecord element = dictionary[file.Path];

                        if (file.ModifiedDate > element.FileModifiedDate)
                        {
                            element.FileModifiedDate = file.ModifiedDate;
                            element.ModifiedDate = DateTime.Now;
                        }
                    }
                }

                harvester.DataContext.SubmitChanges();
                logMessage("Inserted " + insertedRecords + " successful files into DirectoryRecords");
            }
        }
    }
}
