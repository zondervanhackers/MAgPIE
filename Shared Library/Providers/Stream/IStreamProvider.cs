using System;

namespace ZondervanLibrary.SharedLibrary.Providers.Stream
{
    public interface IStreamProvider : IDisposable
    {
        System.IO.Stream Stream { get; }
    }
}
