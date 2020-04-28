using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Meter;

namespace LNF.Impl.Mappings.Meter
{
    internal class MeterDataMap : ClassMap<MeterData>
    {
        internal MeterDataMap()
        {
            Schema("Meter.dbo");
            Id(x => x.MeterDataID);
            Map(x => x.FileIndex);
            Map(x => x.LineIndex);
            Map(x => x.Header);
            Map(x => x.TimeStamp);
            Map(x => x.Value);
        }
    }
}
