using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    internal class PurchaserSearchMap : ClassMap<PurchaserSearch>
    {
        internal PurchaserSearchMap()
        {
            Schema("IOF.dbo");
            Table("v_PurchaserSearch");
            Id(x => x.POID);
            Map(x => x.StatusID);
            Map(x => x.CreatedDate);
            Map(x => x.ClientID);
            Map(x => x.DisplayName);
            Map(x => x.PurchaserID);
            Map(x => x.PurchaserDisplayName);
            Map(x => x.Total);
            Map(x => x.RealPO, "RealPOID");
        }
    }
}
