using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ResourceLabLocationMap:ClassMap<ResourceLabLocation>
    {
        internal ResourceLabLocationMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ResourceLabLocationID);
            Map(x => x.LabLocationID);
            Map(x => x.ResourceID);
        }
    }
}
