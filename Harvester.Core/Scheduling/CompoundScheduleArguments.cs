using System.Xml.Serialization;

namespace ZondervanLibrary.Harvester.Core.Scheduling
{
    [XmlType("CompoundSchedule")]
    public class CompoundScheduleArguments : ScheduleArgumentsBase
    {
        public ScheduleArgumentsBase[] Schedules { get; set; }
    }
}
