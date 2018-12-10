using FluentNHibernate.Mapping;
using LNF.Repository.Meter;

namespace LNF.Impl.Mappings.Meter
{
    internal class MeterReportMap : ClassMap<MeterReport>
    {
        internal MeterReportMap()
        {
            Schema("Meter.dbo");
            Table("Report");
            Id(x => x.ReportID);
            Map(x => x.ReportType);
            Map(x => x.ReportName);
            Map(x => x.Header);
            Map(x => x.UnitCost);
            Map(x => x.BorderColor);
            Map(x => x.BackgroundColor);
            Map(x => x.PointBorderColor);
            Map(x => x.PointBackgroundColor);
            Map(x => x.Active);
        }
    }
}
