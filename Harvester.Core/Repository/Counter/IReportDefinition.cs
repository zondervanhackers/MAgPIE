using ZondervanLibrary.Harvester.Core.Repository.Counter.Sushi_4_1;

namespace ZondervanLibrary.Harvester.Core.Repository.Counter
{
    public interface IReportDefinition
    {
        /// <remarks/>
        ReportDefinitionFilters Filters { get; set; }

        /// <remarks/>
        string Name { get; set; }

        /// <remarks/>
        string Release { get; set; }

        event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}