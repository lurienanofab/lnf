using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    public class StoreBillingByAccountMap : ClassMap<StoreBillingByAccount>
    {
        public StoreBillingByAccountMap()
        {
            ReadOnly();
            Schema("Billing.dbo");
            Table("v_StoreBillingByAccount");
            CompositeId()
                .KeyProperty(x => x.Period)
                .KeyReference(x => x.Client)
                .KeyReference(x => x.Account);
            References(x => x.Org);
            Map(x => x.TotalCharge);
        }
    }
}
