using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    public class RoomDataWithToolUsageMap : ClassMap<RoomDataWithToolUsage>
    {
        public RoomDataWithToolUsageMap()
        {
            Schema("Billing.dbo");
            Table("v_RoomDataWithToolUsage");
            ReadOnly();
            CompositeId()
                .KeyProperty(x => x.Period)
                .KeyProperty(x => x.ClientID)
                .KeyProperty(x => x.RoomID);
            Map(x => x.TotalEntriesPerMonth);
            Map(x => x.TotalHoursPerMonth);
            Map(x => x.PhysicalDays);
        }
    }
}
