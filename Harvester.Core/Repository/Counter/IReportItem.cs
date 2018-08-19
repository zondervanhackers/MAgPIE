using System;
using System.Linq;
using ZondervanLibrary.Harvester.Core.Repository.Counter;
using Sushi = ZondervanLibrary.Harvester.Core.Repository.Counter.Sushi_4_1;
using Enum = System.Enum;

namespace ZondervanLibrary.Harvester.Core.Repository.Counter
{
    public interface IReportItem
    {
        /// <remarks/>
        Sushi.Identifier[] ItemIdentifier { get; set; }

        /// <remarks/>
        string ItemPlatform { get; set; }

        /// <remarks/>
        string ItemName { get; set; }

        /// <remarks/>
        Sushi.DataType ItemDataType { get; set; }

        /// <remarks/>
        Sushi.Metric[] ItemPerformance { get; set; }

        string ItemPublisher { get; set; }

    }
}

namespace ZondervanLibrary.Harvester.Core.Repository.Counter.Sushi_4_1
{
    public partial class ReportItem : IReportItem { }
}

namespace ZondervanLibrary.Harvester.Core.Repository.Journal.Sushi_4_0
{
    public partial class ReportItem : IReportItem
    {
        Sushi.Identifier[] IReportItem.ItemIdentifier
        {
            get
            {
                return ItemIdentifier?.Select(x =>
                    new Sushi.Identifier
                    {
                        Type = ScholarlyIQSushi.ReportItem.ParseEnum<Sushi.IdentifierType>(x.Type.ToString()),
                        Value = x.Value
                    }).ToArray();
            }
            set => throw new NotImplementedException();
        }
        Sushi.DataType IReportItem.ItemDataType
        {
            get => ScholarlyIQSushi.ReportItem.ParseEnum<Sushi.DataType>(ItemDataType.ToString());
            set => throw new NotImplementedException();
        }
        Sushi.Metric[] IReportItem.ItemPerformance
        {
            get
            {
                if (ItemPerformance == null)
                    return new Sushi.Metric[0];

                return ItemPerformance?.Select(x => new Sushi.Metric
                {
                    Instance = x.Instance.Select(y => new Sushi.PerformanceCounter
                    {
                        MetricType = ScholarlyIQSushi.ReportItem.ParseEnum<Sushi.MetricType>(y.MetricType.ToString()),
                        Count = y.Count.ToString()
                    }).ToArray()
                }).ToArray();
            }
            set => throw new NotImplementedException();
        }
    }
}

namespace ZondervanLibrary.Harvester.Core.Repository.Journal.Sushi_3_0
{
    public partial class ReportItem : IReportItem
    {
        Sushi.Identifier[] IReportItem.ItemIdentifier
        {
            get
            {
                return ItemIdentifier?.Select(x =>
                    new Sushi.Identifier
                    {
                        Type = ScholarlyIQSushi.ReportItem.ParseEnum<Sushi.IdentifierType>(x.Type.ToString()),
                        Value = x.Value
                    }).ToArray();
            }
            set => throw new NotImplementedException();
        }
        Sushi.DataType IReportItem.ItemDataType
        {
            get => ScholarlyIQSushi.ReportItem.ParseEnum<Sushi.DataType>(ItemDataType.ToString());
            set => throw new NotImplementedException();
        }
        Sushi.Metric[] IReportItem.ItemPerformance
        {
            get
            {
                if (ItemPerformance == null)
                    return new Sushi.Metric[0];

                return ItemPerformance.Select(x => new Sushi.Metric
                {
                    Instance = x.Instance
                        .Where(z => z.MetricType != MetricType.session_fed && z.MetricType != MetricType.session_reg)
                        .Select(y => new Sushi.PerformanceCounter
                        {
                            MetricType = ScholarlyIQSushi.ReportItem.ParseEnum<Sushi.MetricType>(y.MetricType.ToString()),
                            Count = y.Count.ToString()
                        }).ToArray()
                }).ToArray();
            }
            set => throw new NotImplementedException();
        }
    }
}

namespace ZondervanLibrary.Harvester.Core.MPSInsight
{
    public partial class ReportItem : IReportItem
    {
        Sushi.Identifier[] IReportItem.ItemIdentifier
        {
            get
            {
                return ItemIdentifier.Select(x =>
                    new Sushi.Identifier
                    {
                        Type = ScholarlyIQSushi.ReportItem.ParseEnum<Sushi.IdentifierType>(x.Type.ToString()),
                        Value = x.Value
                    }).ToArray();
            }
            set => throw new NotImplementedException();
        }
        Sushi.DataType IReportItem.ItemDataType
        {
            get => ScholarlyIQSushi.ReportItem.ParseEnum<Sushi.DataType>(ItemDataType.ToString());
            set => throw new NotImplementedException();
        }
        Sushi.Metric[] IReportItem.ItemPerformance {
            get
            {
                if (ItemPerformance == null)
                    return new Sushi.Metric[0];

                return ItemPerformance.Select(x => new Sushi.Metric
                {
                    Instance = x.Instance.Select(y => new Sushi.PerformanceCounter
                    {
                        MetricType = ScholarlyIQSushi.ReportItem.ParseEnum<Sushi.MetricType>(y.MetricType.ToString()),
                        Count = y.Count.ToString()
                    }).ToArray(),
                    // We don't use these fields
                    //Period = new Sushi.DateRange { Begin = x.Period.Begin, End = x.Period.End },
                    //PubYr = x.PubYr,
                    //PubYrFrom = x.PubYrFrom,
                    //PubYrTo = x.PubYrTo
                }).ToArray();
            }
            set => throw new NotImplementedException();
        }
    }
}

namespace ZondervanLibrary.Harvester.Core.ScholarlyIQSushi
{
    public partial class ReportItem : IReportItem
    {
        Sushi.Identifier[] IReportItem.ItemIdentifier
        {
            get
            {
                return ItemIdentifier.Select(x =>
                    new Sushi.Identifier
                    {
                        Type = ParseEnum<Sushi.IdentifierType>(x.Type.ToString()),
                        Value = x.Value
                    }).ToArray();
            }
            set => throw new NotImplementedException();
        }

        Sushi.DataType IReportItem.ItemDataType
        {
            get => ParseEnum<Sushi.DataType>(ItemDataType);
            set => throw new NotImplementedException();
        }

        Sushi.Metric[] IReportItem.ItemPerformance
        {
            get
            {
                if (ItemPerformance == null)
                    return new Sushi.Metric[0];

                return ItemPerformance.Select(x => new Sushi.Metric
                {
                    Instance = x.Instance.Where(z => z.MetricType != "session_reg" && z.MetricType != "session_fed").Select(y => new Sushi.PerformanceCounter
                        {
                            MetricType = ParseEnum<Sushi.MetricType>(y.MetricType),
                            Count = y.Count.ToString()
                        })
                        .ToArray(),
                    // We don't use these fields
                    //Period = new Sushi.DateRange { Begin = x.Period.Begin, End = x.Period.End },
                    //PubYr = x.PubYr,
                    //PubYrFrom = x.PubYrFrom,
                    //PubYrTo = x.PubYrTo
                }).ToArray();
            }
            set => throw new NotImplementedException();
        }

        public static T ParseEnum<T>(string type) where T : struct, IConvertible
        {
            if (Enum.TryParse(type, true, out T resultType)) return resultType;

            throw new java.lang.EnumConstantNotPresentException(typeof(T), type);
        }
    }
}

namespace ZondervanLibrary.Harvester.Core.Gale
{
    public partial class ReportItem : IReportItem
    {
        Sushi.Identifier[] IReportItem.ItemIdentifier
        {
            get
            {
                return ItemIdentifier?.Select(x => new Sushi.Identifier
                {
                    Type = ScholarlyIQSushi.ReportItem.ParseEnum<Sushi.IdentifierType>(x.Type.ToString()),
                    Value = x.Value
                }).ToArray();
            }
            set => throw new NotImplementedException();
        }

        Sushi.DataType IReportItem.ItemDataType
        {
            get => ScholarlyIQSushi.ReportItem.ParseEnum<Sushi.DataType>(ItemDataType.ToString());
            set => throw new NotImplementedException();
        }

        Sushi.Metric[] IReportItem.ItemPerformance
        {
            get
            {
                if (ItemPerformance == null)
                    return new Sushi.Metric[0];

                return ItemPerformance?.Select(x => new Sushi.Metric
                {
                    Instance = x.Instance.Select(y => new Sushi.PerformanceCounter
                    {
                        MetricType = ScholarlyIQSushi.ReportItem.ParseEnum<Sushi.MetricType>(y.MetricType.ToString()),
                        Count = y.Count.ToString()
                    }).ToArray(),
                    // We don't use these fields
                    //Period = new Sushi.DateRange { Begin = x.Period.Begin, End = x.Period.End },
                    //PubYr = x.PubYr,
                    //PubYrFrom = x.PubYrFrom,
                    //PubYrTo = x.PubYrTo
                }).ToArray();
            }
            set => throw new NotImplementedException();
        }
    }
}