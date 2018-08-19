using System;

namespace ZondervanLibrary.SharedLibrary.Providers.Stream
{
    public interface IDirectoryProvider
    {
        System.IO.Stream CreateStream(String path);
    }
}
