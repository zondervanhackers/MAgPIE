using System;

namespace ZondervanLibrary.SharedLibrary.Providers.Stream
{
    public class StreamProvider : IStreamProvider
    {
        private readonly System.IO.Stream _stream;

        public StreamProvider(IDirectoryProvider directoryProvider, String filePath)
        {
            _stream = directoryProvider.CreateStream(filePath);
        }

        public System.IO.Stream Stream => _stream;

        public void Dispose()
        {
            _stream.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
