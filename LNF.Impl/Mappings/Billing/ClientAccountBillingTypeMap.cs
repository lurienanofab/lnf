using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Billing
{
    public class ClientAccountBillingTypeMap : ClassMap<LNF.Repository.Billing.ClientAccountBillingType>
    {
        public ClientAccountBillingTypeMap()
        {
            Id(x => x.BillingTypeID);
        }
    }
}
