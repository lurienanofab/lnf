using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    public class PurchaserMap:ClassMap<Purchaser>
    {
        public PurchaserMap()
        {
            Schema("IOF.dbo");
            Id(x => x.PurchaserID);
            References(x => x.Client);
            Map(x => x.Active);
            Map(x => x.Deleted);
        }
    }
}
