using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace ZondervanLibrary.SharedLibrary.Collections
{
    [DataContract]
    public class SerializableNotifyCollectionChangedEventArgs
    {
        public SerializableNotifyCollectionChangedEventArgs(NotifyCollectionChangedEventArgs args)
        {
            Debug.Assert(args != null);

            Action = args.Action;
            NewItems = args.NewItems;
            NewStartingIndex = args.NewStartingIndex;
            OldItems = args.OldItems;
            OldStartingIndex = args.OldStartingIndex;
        }

        [DataMember]
        public NotifyCollectionChangedAction Action { get; protected set; }

        [DataMember]
        public IList NewItems { get; protected set; }

        [DataMember]
        public Int32 NewStartingIndex { get; protected set; }

        [DataMember]
        public IList OldItems { get; protected set; }

        [DataMember]
        public Int32 OldStartingIndex { get; protected set; }
    }
}
