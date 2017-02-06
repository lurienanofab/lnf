using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Reporting;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Reporting
{
    public class ToolUtilizationMap : ClassMap<ToolUtilization>
    {
        public ToolUtilizationMap()
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
