namespace ZondervanLibrary.Harvester.Core.Repository.Counter
{
    public interface IRequestor
    {
        /// <remarks/>
        string ID { get; set; }

        /// <remarks/>
        string Name { get; set; }

        /// <remarks/>
        string Email { get; set; }

        event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}