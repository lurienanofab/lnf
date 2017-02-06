using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    public class RoomBillingByRoomOrgMap : ClassMap<RoomBillingByRoomOrg>
    {
        public RoomBillingByRoomOrgMap()
        {
            ReadOnly();
            Schema("Billing.dbo");
            Table("v_RoomBillingByRoomOrg");
            CompositeId()
                .KeyProperty(x => x.Period)
                .KeyReference(x => x.Client)
                .KeyReference(x => x.Org)
                .KeyReference(x => x.Room);
            References(x => x.ChargeType);
            References(x => x.BillingType);
            Map(x => x.ChargeDays);
            Map(x => x.Entries);
            Map(x => x.Hours);
            Map(x => x.RoomCharge);
            Map(x => x.EntryCharge);
            Map(x => x.SubsidyDiscount);
            Map(x => x.TotalCharge);
        }
    }
}
