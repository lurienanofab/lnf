using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class MiscBillingChargeMap : ClassMap<MiscBillingCharge>
    {
        public MiscBillingChargeMap()
        {
            Schema("sselData.dbo");
            Id(x => x.ExpID);
            References(x => x.Client);
            References(x => x.Account);
            Map(x => x.SUBType);
            Map(x => x.Period);
            Map(x => x.ActDate);
            Map(x => x.Description);
            Map(x => x.Quantity);
            Map(x => x.UnitCost);
            Map(x => x.SubsidyDiscount);
            Map(x => x.Active);
        }
    }
}
