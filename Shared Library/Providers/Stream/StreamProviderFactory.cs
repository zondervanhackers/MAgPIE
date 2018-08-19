using System;

namespace ZondervanLibrary.SharedLibrary.Providers.Stream
{
    public class StreamProviderFactory : IStreamProviderFactory
    {
        private readonly IDirectoryProvider _directoryProvider;
        private readonly String _filePathFormat;

        public StreamProviderFactory(IDirectoryProvider directoryProvider, String filePathFormat)
        {
            _directoryProvider = directoryProvider;
            _filePathFormat = filePathFormat;
        }

        public IStreamProvider CreateInstance(DateTime runDate)
        {
            return new StreamProvider(_directoryProvider, String.Format(_filePathFormat, runDate));
        }
    }
}
