using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    internal class ReceivingMap : ClassMap<Receiving>
    {
        internal ReceivingMap()
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
