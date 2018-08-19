using System;
using System.IO;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using FileHelpers;

using ZondervanLibrary.PatronTranslator.Console.IO;

namespace ZondervanLibrary.PatronTranslator.Console.Repository
{
    public class FlatRepository<TEntity> : ConsumingRepository<TEntity>
        where TEntity : class
    {
        private FileHelperEngine _fileHelperEngine;
        private IStreamFactory _streamFactory;

        public FlatRepository(IStreamFactory streamFactory)
        {
            Initialize(streamFactory);

            _dataSource = null;
        }

        public FlatRepository(IStreamFactory streamFactory, IList<TEntity> dataSource)
        {
            Initialize(streamFactory);

            _dataSource = dataSource;
        }

        protected override void PopulateDataSource()
        {
            using (Stream stream = _streamFactory.CreateInstance(StreamMode.Read))
            {
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    _dataSource = ((TEntity[])_fileHelperEngine.ReadStream(streamReader)).ToList();
                }
            }
        }

        private void Initialize(IStreamFactory streamFactory)
        {
            _streamFactory = streamFactory ?? throw new ArgumentNullException(nameof(streamFactory));

            _fileHelperEngine = new FileHelperEngine(typeof(TEntity));
        }

        public override void SubmitChanges()
        {
            SubmitChanges(ConflictMode.FailOnFirstConflict);
        }

        public override void SubmitChanges(ConflictMode failureMode)
        {
            ApplyChanges(failureMode);

            using (Stream stream = _streamFactory.CreateInstance(StreamMode.Write))
            {
                using (StreamWriter streamWriter = new StreamWriter(stream))
                {
                    _fileHelperEngine.WriteStream(streamWriter, _dataSource);

                    streamWriter.Flush();
                }
            }
        }
    }
}
