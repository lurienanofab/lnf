using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    public class ToolBillingByToolOrgMap : ClassMap<ToolBillingByToolOrg>
    {
        public ToolBillingByToolOrgMap()
        {
            ReadOnly();
            Schema("Billing.dbo");
            Table("v_ToolBillingByToolOrg");
            CompositeId()
                .KeyProperty(x => x.Period)
                .KeyReference(x => x.Client)
                .KeyReference(x => x.Org)
                .KeyReference(x => x.Resource);
            References(x => x.ChargeType);
            References(x => x.BillingType);
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
