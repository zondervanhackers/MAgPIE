using System;
using System.Collections.Specialized;
using System.ServiceModel;
using ZondervanLibrary.Harvester.Communication;
using ZondervanLibrary.SharedLibrary.Collections;

namespace ZondervanLibrary.Harvester.Wpf.Communication
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class HarvesterClientConnection : IHarvesterClientConnection
    {
        public void OnRunningOperationsCollectionChanged(SerializableNotifyCollectionChangedEventArgs eventArgs)
        {
            CollectionChanged(OnRunningOperationsCollectionChangedCallback, eventArgs);
        }

        public void OnCancelledOperationsCollectionChanged(SerializableNotifyCollectionChangedEventArgs eventArgs)
        {
            CollectionChanged(OnCancelledOperationsCollectionChangedCallback, eventArgs);
        }

        private void CollectionChanged(Action<NotifyCollectionChangedEventArgs> callback, SerializableNotifyCollectionChangedEventArgs eventArgs)
        {
            if (OnRunningOperationsCollectionChangedCallback != null)
            {
                NotifyCollectionChangedEventArgs args;

                switch (eventArgs.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, eventArgs.NewItems, eventArgs.NewStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, eventArgs.OldItems, eventArgs.OldStartingIndex);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                callback(args);
            }
        }

        public Action<NotifyCollectionChangedEventArgs> OnRunningOperationsCollectionChangedCallback { get; set; }

        public Action<NotifyCollectionChangedEventArgs> OnCancelledOperationsCollectionChangedCallback { get; set; }
    }
}
