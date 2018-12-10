using FluentNHibernate.Mapping;
using LNF.Repository.Meter;

namespace LNF.Impl.Mappings.Meter
{
    public class MeterDataMap : ClassMap<MeterData>
    {
        public MeterDataMap()
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
