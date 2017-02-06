using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    public class StoreBillingByItemOrgMap : ClassMap<StoreBillingByItemOrg>
    {
        public StoreBillingByItemOrgMap()
        {
            ReadOnly();
            Schema("Billing.dbo");
            Table("v_StoreBillingByItemOrg");
            CompositeId()
                .KeyProperty(x => x.Period)
                .KeyReference(x => x.Client)
                .KeyReference(x => x.Org)
                .KeyReference(x => x.Item);
            References(x => x.ChargeType);
            Map(x => x.TotalCharge);
        }
    }
}
