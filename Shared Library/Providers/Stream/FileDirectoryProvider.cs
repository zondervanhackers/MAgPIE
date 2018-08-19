using System;
using System.IO;

namespace ZondervanLibrary.SharedLibrary.Providers.Stream
{
    public class FileDirectoryProvider : IDirectoryProvider
    {
        private readonly DirectoryInfo _directoryInfo;

        public FileDirectoryProvider(String directoryPath)
        {
            _directoryInfo = new DirectoryInfo(directoryPath);
        }

        public System.IO.Stream CreateStream(String path)
        {
            return new FileStream($@"{_directoryInfo.FullName}\{path}", FileMode.OpenOrCreate);
        }
    }
}
