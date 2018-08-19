using System.Collections.Generic;
using System.ServiceModel;
using OperationContext = ZondervanLibrary.Harvester.Core.Operations.OperationContext;

namespace ZondervanLibrary.Harvester.Communication
{
    [ServiceContract(SessionMode = SessionMode.Allowed, CallbackContract = typeof(IHarvesterClientConnection))]
    public interface IHarvesterServiceConnection
    {
        /// <summary>
        /// Subscribes the client to the service.  This will allow the service to send notifications to the client.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void Subscribe();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<OperationContext> RunningOperations();
    }
}
