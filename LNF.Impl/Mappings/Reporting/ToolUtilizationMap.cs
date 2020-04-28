using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Reporting;

namespace LNF.Impl.Mappings.Reporting
{
    internal class ToolUtilizationMap : ClassMap<ToolUtilization>
    {
        internal ToolUtilizationMap()
        {
            Schema("Reporting.dbo");
            Table("v_ToolUtilization");
            ReadOnly();
            CompositeId().KeyProperty(x => x.Period)
                .KeyProperty(x => x.ResourceID)
                .KeyProperty(x => x.ActivityID);
            Map(x => x.ResourceName);
            Map(x => x.ProcessTechID);
            Map(x => x.ProcessTechName);
            Map(x => x.ActivityName);
            Map(x => x.TotalActDurationHours);
            Map(x => x.TotalChargeDurationHours);
            Map(x => x.TotalChargeDurationForgivenHours);
            Map(x => x.TotalTransferredDurationHours);
            Map(x => x.TotalSchedDurationHours);
            Map(x => x.TotalOverTimeHours);
        }
    }
}
