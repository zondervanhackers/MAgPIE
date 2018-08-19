namespace ZondervanLibrary.Harvester.Core.Repository.Counter
{
    public interface ICustomerReference
    {
        /// <remarks/>
        string ID { get; set; }

        /// <remarks/>
        string Name { get; set; }

        event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}