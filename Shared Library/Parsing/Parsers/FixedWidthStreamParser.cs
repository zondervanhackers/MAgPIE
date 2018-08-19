using System;
using System.Collections.Generic;
using System.IO;

namespace ZondervanLibrary.SharedLibrary.Parsing.Parsers
{
    public class FixedWidthStreamParser<TRecord> : IStreamParser<TRecord>
        where TRecord : new()
    {
        public IEnumerable<TRecord> ParseStream(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void WriteStream(Stream stream, IEnumerable<TRecord> items)
        {
            throw new NotImplementedException();
        }
    }
}
