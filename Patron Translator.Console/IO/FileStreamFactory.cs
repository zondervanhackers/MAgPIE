using System;
using System.IO;

namespace ZondervanLibrary.PatronTranslator.Console.IO
{
    public class FileStreamFactory : IStreamFactory
    {
        private readonly String _filePath;

        public FileStreamFactory(String filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public Stream CreateInstance(StreamMode streamMode)
        {
            if (!Enum.IsDefined(typeof(StreamMode), streamMode))
                throw new ArgumentOutOfRangeException(nameof(streamMode));

            switch (streamMode)
            {
                case StreamMode.Read:
                    return File.OpenRead(_filePath);
                case StreamMode.Write:
                    return File.OpenWrite(_filePath);
                default:
                    throw new ArgumentOutOfRangeException(nameof(streamMode));
            }
        }
    }
}
