using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    public class BuildingMap : ClassMap<Building>
    {
        public BuildingMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.BuildingID);
            Map(x => x.BuildingName);
            Map(x => x.Description);
            Map(x => x.IsActive);
        }
    }
}
