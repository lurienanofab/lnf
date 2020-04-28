using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    internal class PurchaseOrderInfoMap : ClassMap<PurchaseOrderInfo>
    {
        internal PurchaseOrderInfoMap()
        {
            Schema("IOF.dbo");
            Table("v_PurchaseOrderInfo");
            ReadOnly();
            Id(x => x.POID);
            Map(x => x.ClientID).Not.Nullable();
            Map(x => x.DisplayName).Not.Nullable();
            Map(x => x.VendorID).Not.Nullable();
            Map(x => x.VendorName).Not.Nullable();
            Map(x => x.AccountID).Nullable();
            Map(x => x.ShortCode).Nullable();
            Map(x => x.ApproverID).Not.Nullable();
            Map(x => x.ApproverName).Not.Nullable();
            Map(x => x.CreatedDate).Not.Nullable();
            Map(x => x.NeededDate).Not.Nullable();
            Map(x => x.Oversized).Not.Nullable();
            Map(x => x.ShippingMethodID).Not.Nullable();
            Map(x => x.ShippingMethodName).Not.Nullable();
            Map(x => x.Notes).Nullable();
            Map(x => x.StatusID).Not.Nullable();
            Map(x => x.StatusName).Not.Nullable();
            Map(x => x.CompletedDate).Nullable();
            Map(x => x.RealApproverID).Nullable();
            Map(x => x.ApprovalDate).Nullable();
            Map(x => x.Attention).Not.Nullable();
            Map(x => x.PurchaserID).Nullable();
            Map(x => x.RealPO).Nullable();
            Map(x => x.ReqNum).Nullable();
            Map(x => x.PurchaserNotes).Nullable();
            Map(x => x.TotalPrice).Not.Nullable();
        }
    }
}
