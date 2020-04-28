namespace LNF.Ordering
{
    public interface IOrderingService
    {
        IPurchaseOrderRepository PurchaseOrder { get; }
        IPurchaseOrderItemRepository Item { get; }
        IPurchaseOrderCategoryRepository Category { get; }
        IVendorRepository Vendor { get; }
        IApproverRepository Approver { get; }
        IPurchaserRepository Purchaser { get; }
        ITrackingRepository Tracking { get; }
    }
}
