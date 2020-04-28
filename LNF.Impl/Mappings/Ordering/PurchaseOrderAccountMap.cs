using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    internal class PurchaseOrderAccountMap : ClassMap<PurchaseOrderAccount>
    {
        internal PurchaseOrderAccountMap()
        {
            Schema("IOF.dbo");
            Table("Account");
            CompositeId()
                .KeyProperty(x => x.AccountID)
                .KeyProperty(x => x.ClientID);
            Map(x => x.Active);
        }
    }
}
