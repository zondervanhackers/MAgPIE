using System.ServiceModel;
using System.ServiceModel.Channels;

using ZondervanLibrary.Harvester.Core.Repository.Counter.Sushi_4_1;
using ZondervanLibrary.SharedLibrary.Factory;

namespace ZondervanLibrary.Harvester.Core.Repository.Counter
{
    public class SushiServiceInterfaceClientFactory : IFactory<ISushiServiceInterfaceClient, EndpointAddress>
    {
        private readonly Binding _binding;

        public SushiServiceInterfaceClientFactory(Binding binding)
        {
            _binding = binding;
        }

        public ISushiServiceInterfaceClient CreateInstance(EndpointAddress endpoint)
        {
            return new SushiServiceInterfaceClient(_binding, endpoint);
        }
    }
}
