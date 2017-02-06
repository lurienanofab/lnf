using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Billing;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Billing
{
    public class StoreBillingMap : ClassMap<StoreBilling>
    {
        public StoreBillingMap()
        {
            Schema("sselData.dbo");
            Table("StoreBilling");
            Id(x => x.StoreBillingID);
            Map(x => x.Period);
            Map(x => x.ClientID);
            Map(x => x.AccountID);
            Map(x => x.ChargeTypeID);
            Map(x => x.ItemID);
            Map(x => x.Quantity);
            Map(x => x.UnitCost);
            Map(x => x.CategoryID);
            Map(x => x.CostMultiplier);
            Map(x => x.LineCost);
            Map(x => x.StatusChangeDate);
        }
    }
}
