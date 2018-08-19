using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using ZondervanLibrary.SharedLibrary.Factory;

using ZondervanLibrary.SharedLibrary.Parsing.Records;
using ZondervanLibrary.SharedLibrary.Parsing.Parsers;

namespace ZondervanLibrary.SharedLibrary.Parsing
{
    public class StreamParserFactory<TRecord> : IFactory<IStreamParser<TRecord>>
        where TRecord : new()
    {
        private readonly IRecordAttribute _attribute;

        public StreamParserFactory()
        {
            IEnumerable<IRecordAttribute> attributes = typeof(TRecord).GetCustomAttributes().Where(a => typeof(IRecordAttribute).IsAssignableFrom(a.GetType())).Cast<IRecordAttribute>();

            if (!attributes.Any())
            {
                throw new ArgumentException("TRecord must have a record attribute in order to create a stream parser.");
            }
            else if (attributes.Count() > 1)
            {
                throw new ArgumentException("TRecord cannot have multiple record attributes.");
            }

            _attribute = attributes.First();
        }

        public IStreamParser<TRecord> CreateInstance()
        {
            if (_attribute.GetType() == typeof(DelimitedRecordAttribute))
            {
                return new DelimitedStreamParser<TRecord>();
            }
            else if (_attribute.GetType() == typeof(FixedWidthRecordAttribute))
            {
                return new FixedWidthStreamParser<TRecord>();
            }

            throw new NotImplementedException();
        }
    }
}
