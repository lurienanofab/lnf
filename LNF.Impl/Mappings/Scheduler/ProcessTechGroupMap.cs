using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    public class ProcessTechGroupMap : ClassMap<ProcessTechGroup>
    {
        public ProcessTechGroupMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ProcessTechGroupID);
            Map(x => x.GroupName);
        }
    }
}
