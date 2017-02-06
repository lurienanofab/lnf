using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    public class StoreBillingByOrgMap : ClassMap<StoreBillingByOrg>
    {
        public StoreBillingByOrgMap()
        {
            ReadOnly();
            Schema("Billing.dbo");
            Table("v_StoreBillingByOrg");
            CompositeId()
                .KeyProperty(x => x.Period)
                .KeyReference(x => x.Client)
                .KeyReference(x => x.Org);
            Map(x => x.TotalCharge);
        }
    }
}
