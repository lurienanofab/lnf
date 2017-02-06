using FluentNHibernate.Mapping;
using LNF.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    internal class PurchaseOrderSearchMap : ClassMap<PurchaseOrderSearch>
    {
        internal PurchaseOrderSearchMap()
        {
            Schema("IOF.dbo");
            Table("v_PurchaseOrderSearch");
            ReadOnly();
            Id(x => x.PODID);
            Map(x => x.POID);
            Map(x => x.ItemID);
            Map(x => x.StatusID);
            Map(x => x.StatusName);
            Map(x => x.CreatedDate);
            Map(x => x.ClientID);
            Map(x => x.DisplayName);
            Map(x => x.ApproverID);
            Map(x => x.ApproverDisplayName);
            Map(x => x.VendorID);
            Map(x => x.VendorName);
            Map(x => x.CleanVendorName);
            Map(x => x.AccountID);
            Map(x => x.AccountName);
            Map(x => x.ShortCode);
            Map(x => x.ItemCount);
            Map(x => x.TotalPrice);
            Map(x => x.PartNum);
            Map(x => x.Description);
        }
    }
}
