using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    internal class PurchaseOrderMap : ClassMap<PurchaseOrder>
    {
        internal PurchaseOrderMap()
        {
            Schema("IOF.dbo");
            Id(x => x.POID);
            References(x => x.Client).Not.Nullable();
            References(x => x.Vendor).Not.Nullable();
            Map(x => x.AccountID).Nullable();
            References(x => x.Approver).Not.Nullable();
            Map(x => x.CreatedDate).Not.Nullable();
            Map(x => x.NeededDate).Not.Nullable();
            Map(x => x.Oversized).Not.Nullable();
            References(x => x.ShippingMethod).Not.Nullable();
            Map(x => x.Notes).Nullable();
            References(x => x.Status).Not.Nullable();
            Map(x => x.CompletedDate).Nullable();
            Map(x => x.RealApproverID).Nullable();
            Map(x => x.ApprovalDate).Nullable();
            Map(x => x.Attention).Not.Nullable();
            Map(x => x.PurchaserID).Nullable();
            Map(x => x.RealPO, "RealPOID").Nullable();
            Map(x => x.ReqNum).Nullable();
            Map(x => x.PurchaserNotes).Nullable();
            HasMany(x => x.Details).KeyColumn("POID").Inverse().NotFound.Ignore();
        }
    }
}
