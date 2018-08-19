using System;

namespace ZondervanLibrary.SharedLibrary.Providers.Stream
{
    public interface IStreamProviderFactory
    {
        IStreamProvider CreateInstance(DateTime runDate);
    }
}
