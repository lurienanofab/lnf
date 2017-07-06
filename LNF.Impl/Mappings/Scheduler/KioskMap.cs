using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class KioskMap : ClassMap<Kiosk>
    {
        internal KioskMap()
        {
            Schema("sselScheduler.dbo");
            Table("Kiosk");
            Id(x => x.KioskID);
            Map(x => x.KioskName);
            Map(x => x.KioskIP);
        }
    }
}
