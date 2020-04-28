using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Reporting;

namespace LNF.Impl.Mappings.Reporting
{
    internal class AfterHoursMap : ClassMap<AfterHours>
    {
        internal AfterHoursMap()
        {
            Schema("Reporting.dbo");
            Table("AfterHours");
            Id(x => x.AfterHoursID);
            Map(x => x.AfterHoursName);
            Map(x => x.DayOfWeekIndex);
            Map(x => x.HourIndex);
            Map(x => x.IsAfterHours);
        }
    }
}
