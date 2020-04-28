using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ProcessTechMap : ClassMap<ProcessTech>
    {
        internal ProcessTechMap()
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
