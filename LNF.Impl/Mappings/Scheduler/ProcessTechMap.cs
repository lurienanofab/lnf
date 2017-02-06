using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    public class ProcessTechMap : ClassMap<ProcessTech>
    {
        public ProcessTechMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ProcessTechID);
            References(x => x.Group, "ProcessTechGroupID");
            References(x => x.Lab, "LabID");
            Map(x => x.ProcessTechName);
            Map(x => x.Description);
            Map(x => x.IsActive);
        }
    }
}
