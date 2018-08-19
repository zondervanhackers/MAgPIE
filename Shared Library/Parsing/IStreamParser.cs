using System.Collections.Generic;
using System.IO;

namespace ZondervanLibrary.SharedLibrary.Parsing
{
    public interface IStreamParser<TRecord>
        where TRecord : new()
    {
        IEnumerable<TRecord> ParseStream(Stream stream);

        void WriteStream(Stream stream, IEnumerable<TRecord> items);
    }
}
