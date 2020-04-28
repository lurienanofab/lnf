using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Meter;

namespace LNF.Impl.Mappings.Meter
{
    internal class MeterServiceLogMap : ClassMap<MeterServiceLog>
    {
        internal MeterServiceLogMap()
        {
            Schema("Meter.dbo");
            Table("ServiceLog");
            Id(x => x.ServiceLogID);
            Map(x => x.LogMessageType);
            Map(x => x.LogMessage);
            Map(x => x.EntryDateTime);
            Map(x => x.EntryData);
        }
    }
}
