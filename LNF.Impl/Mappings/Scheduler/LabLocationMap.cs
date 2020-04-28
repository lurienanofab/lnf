using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class LabLocationMap:ClassMap<LabLocation>
    {
        internal LabLocationMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.LabLocationID);
            Map(x => x.LabID);
            Map(x => x.LocationName);
        }
    }
}
