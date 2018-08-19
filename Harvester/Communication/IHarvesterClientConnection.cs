using System.ServiceModel;
using ZondervanLibrary.SharedLibrary.Collections;

namespace ZondervanLibrary.Harvester.Communication
{
    [ServiceContract]
    public interface IHarvesterClientConnection
    {
        [OperationContract(IsOneWay = true)]
        void OnRunningOperationsCollectionChanged(SerializableNotifyCollectionChangedEventArgs eventArgs);

        [OperationContract(IsOneWay = true)]
        void OnCancelledOperationsCollectionChanged(SerializableNotifyCollectionChangedEventArgs eventArgs);

        //[OperationContract]
        //void OnExecutingCollectionChanged();
    }
}
