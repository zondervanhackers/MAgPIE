using System.IO;

namespace ZondervanLibrary.PatronTranslator.Console.IO
{
    public enum StreamMode
    {
        Read = 0x00000001,
        Write = 0x00000002
    }

    /// <summary>
    /// When implemented in a derived class CreateInstance should provide a Stream that either supports reading or writing.
    /// </summary>
    public interface IStreamFactory
    {
        Stream CreateInstance(StreamMode streamMode);
    }
}
