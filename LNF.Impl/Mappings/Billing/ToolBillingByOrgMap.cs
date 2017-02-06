using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    public class ToolBillingByOrgMap : ClassMap<ToolBillingByOrg>
    {
        public ToolBillingByOrgMap()
        {
            ReadOnly();
            Schema("Billing.dbo");
            Table("v_ToolBillingByOrg");
            CompositeId()
                .KeyProperty(x => x.Period)
                .KeyReference(x => x.Client)
                .KeyReference(x => x.Org);
            Map(x => x.UsageFeeCharged);
            Map(x => x.OverTimePenaltyFee);
            Map(x => x.UncancelledPenaltyFee);
            Map(x => x.ReservationFee);
            Map(x => x.BookingFee);
            Map(x => x.ForgivenFee);
            Map(x => x.TransferredFee);
            Map(x => x.SubsidyDiscount);
            Map(x => x.TotalCharge);
        }
    }
}
