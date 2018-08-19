using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Reflection;

using ZondervanLibrary.PatronTranslator.Console.IO;

namespace ZondervanLibrary.PatronTranslator.Console.Repository
{
    public class XmlRepository<TEntity, TRootNode> : ConsumingRepository<TEntity>
        where TEntity : class
        where TRootNode : class, new()
    {
        private IStreamFactory _streamFactory;
        //private XmlSerializer _serializer;

        private TRootNode _rootNode;
        private PropertyInfo _childNodesProperty;

        public XmlRepository(IStreamFactory streamFactory)
        {
            Initialize(streamFactory);
        }

        /// <summary>
        /// A constructor for XmlRepository that sets the initial source collection to the passed list instead of parsing from the stream.
        /// </summary>
        /// <param name="stream">The stream to write to on SubmitChanges.</param>
        /// <param name="initialSource">The initial collection of entities.</param>
        public XmlRepository(IStreamFactory streamFactory, IList<TEntity> initialSource)
        {
            Initialize(streamFactory);

            _dataSource = initialSource ?? throw new ArgumentNullException(nameof(initialSource));
            _rootNode = new TRootNode();
        }

        protected override void PopulateDataSource()
        {
            using (Stream stream = _streamFactory.CreateInstance(StreamMode.Read))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TRootNode));
                _rootNode = (TRootNode)serializer.Deserialize(stream);
                _dataSource = ((TEntity[])_childNodesProperty.GetValue(_rootNode)).ToList();
            }
        }

        private void Initialize(IStreamFactory streamFactory)
        {
            _streamFactory = streamFactory ?? throw new ArgumentNullException(nameof(streamFactory));

            _childNodesProperty = typeof(TRootNode).GetProperties().Single(p => p.PropertyType == typeof(TEntity[]));
        }

        public override void SubmitChanges()
        {
            SubmitChanges(ConflictMode.FailOnFirstConflict);
        }

        public override void SubmitChanges(ConflictMode failureMode)
        {
            ApplyChanges(failureMode);

            _childNodesProperty.SetValue(_rootNode, _dataSource.ToArray());

            using (Stream stream = _streamFactory.CreateInstance(StreamMode.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TRootNode));
                serializer.Serialize(stream, _rootNode);
            }
        }
    }
}
