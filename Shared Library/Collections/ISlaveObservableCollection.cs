using System.Collections.Generic;
using System.Collections.Specialized;

namespace ZondervanLibrary.SharedLibrary.Collections
{
    //[ContractClass(typeof(ISlaveObservableCollectionContract<>))]
    public interface ISlaveObservableCollection<T> : INotifyCollectionChanged, IReadOnlyList<T>
    {
        void OnMasterCollectionChanged(NotifyCollectionChangedEventArgs eventArgs);
    }

    //[ContractClassFor(typeof(ISlaveObservableCollection<>))]
    //internal abstract class ISlaveObservableCollectionContract<T> : ISlaveObservableCollection<T>
    //{
    //    void ISlaveObservableCollection<T>.OnMasterCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
    //    {
    //        Contract.Requires(eventArgs != null);

    //        // Although NewItems and OldItems are lists, the implementation of
    //        // ObservableCollection only allows for one element to be changed at a time.
    //        // Also the WPF default CollectionView class does not support ranged actions
    //        // so we only support the single valued cases here.
    //        Contract.Requires(eventArgs.NewItems.Count < 2);
    //        Contract.Requires(eventArgs.OldItems.Count < 2);
    //    }
    //}
}