using System;
using System.ComponentModel;
using ZondervanLibrary.Harvester.Core.Repository.Counter;

namespace ZondervanLibrary.Harvester.Core.Repository.Counter
{
    public interface IRange
    {
        /// <remarks/>
        DateTime Begin { get; set; }

        /// <remarks/>
        DateTime End { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }

    
}

namespace ZondervanLibrary.Harvester.Core.Repository.Counter.Sushi_4_1
{
    public partial class Range : IRange { }
}

namespace ZondervanLibrary.Harvester.Core.Repository.Journal.Sushi_3_0
{
    public partial class Range : IRange { }
}

namespace ZondervanLibrary.Harvester.Core.ScholarlyIQSushi
{
    public partial class Range : IRange { }
}