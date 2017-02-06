using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    public class ReceivingMap : ClassMap<Receiving>
    {
        public ReceivingMap()
        {
            Schema("IOF.dbo");
            Id(x => x.ReceivingID);
            References(x => x.PurchaseOrderDetail, "PODID");
            References(x => x.Client);
            Map(x => x.Quantity);
            Map(x => x.ReceivedDate);
        }
    }
}
