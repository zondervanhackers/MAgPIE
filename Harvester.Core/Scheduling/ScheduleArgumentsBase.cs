using System.Xml.Serialization;

namespace ZondervanLibrary.Harvester.Core.Scheduling
{
    [XmlType("Schedule")]
    [XmlInclude(typeof(IntervalScheduleArguments))]
    public abstract class ScheduleArgumentsBase
    {

    }
}
