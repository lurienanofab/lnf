using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class BuildingMap : ClassMap<Building>
    {
        internal BuildingMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.BuildingID);
            Map(x => x.BuildingName);
            Map(x => x.BuildingDescription, "Description");
            Map(x => x.BuildingIsActive, "IsActive");
        }
    }
}
