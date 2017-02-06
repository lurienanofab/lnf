using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Store;

namespace LNF.Impl.Mappings.Store
{
    public class StoreOrderDetailMap : ClassMap<StoreOrderDetail>
    {
        public StoreOrderDetailMap()
        {
            Schema("sselMAS.dbo");
            Id(x => x.SODID);
            References(x => x.StoreOrder, "SOID");
            References(x => x.Item);
            Map(x => x.Quantity);
            Map(x => x.PriceID);
        }
    }
}
