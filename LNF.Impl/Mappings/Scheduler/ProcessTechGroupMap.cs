using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ProcessTechGroupMap : ClassMap<ProcessTechGroup>
    {
        internal ProcessTechGroupMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ProcessTechGroupID);
            Map(x => x.GroupName);
        }
    }
}
