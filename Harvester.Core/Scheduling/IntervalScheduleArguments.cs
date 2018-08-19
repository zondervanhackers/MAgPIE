using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ZondervanLibrary.Harvester.Core.Scheduling
{
    [XmlType("IntervalSchedule")]
    public class IntervalScheduleArguments : ScheduleArgumentsBase
    {
        [XmlAttribute]
        public Double Interval { get; set; }

        [XmlAttribute]
        public IntervalUnit Unit { get; set; }

        [XmlAttribute]
        public DateTime StartDate { get; set; }

        [XmlIgnore]
        public DateTime? EndDate { get; set; }

        // Some trickery to allow EndDate to be a nullable attribute
        [XmlAttribute("EndDate")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public DateTime EndDateSerialized
        {
            get => EndDate.Value;
            set => EndDate = value;
        }

        // Some trickery to all EndDate to be a nullable attribute
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeEndDateSerialized()
        {
            return EndDate.HasValue;
        }

        public bool EndDateSpecified()
        {
            return EndDate.HasValue;
        }
    }
}
