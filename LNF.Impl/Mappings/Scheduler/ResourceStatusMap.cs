using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ResourceStatusMap: ClassMap<ResourceStatus>
    {
        internal ResourceStatusMap()
        {
            Schema("sselScheduler.dbo");
            Table("v_ResourceStatus");
            ReadOnly();
            Id(x => x.ResourceID);
            Map(x => x.ResourceName);
            Map(x => x.ResourceIsActive);
            Map(x => x.Available);
            Map(x => x.ActiveLName);
            Map(x => x.ActiveFName);
            Map(x => x.ActiveBeginDateTime);
            Map(x => x.ActiveEndDateTime);
            Map(x => x.UpcomingLName);
            Map(x => x.UpcomingFName);
            Map(x => x.UpcomingBeginDateTime);
            Map(x => x.UpcomingEndDateTime);
        }
    }
}
