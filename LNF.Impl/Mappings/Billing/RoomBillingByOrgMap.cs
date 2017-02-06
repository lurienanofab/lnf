using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    public class RoomBillingByOrgMap : ClassMap<RoomBillingByOrg>
    {
        public RoomBillingByOrgMap()
        {
            ReadOnly();
            Schema("Billing.dbo");
            Table("v_RoomBillingByOrg");
            CompositeId()
                .KeyProperty(x => x.Period)
                .KeyReference(x => x.Client)
                .KeyReference(x => x.Org);
            Map(x => x.RoomCharge);
            Map(x => x.EntryCharge);
            Map(x => x.SubsidyDiscount);
            Map(x => x.TotalCharge);
        }
    }
}
