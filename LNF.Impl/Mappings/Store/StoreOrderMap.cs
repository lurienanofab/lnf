using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Store;

namespace LNF.Impl.Mappings.Store
{
    public class StoreOrderMap:ClassMap<StoreOrder>
    {
        public StoreOrderMap()
        {
            Schema("sselMAS.dbo");
            Id(x => x.SOID);
            References(x => x.Client);
            References(x => x.Account);
            Map(x => x.CreationDate);
            Map(x => x.Status);
            Map(x => x.StatusChangeDate);
            Map(x => x.InventoryLocationID);
        }
    }
}
