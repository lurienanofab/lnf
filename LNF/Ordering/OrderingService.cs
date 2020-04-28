namespace LNF.Ordering
{
    public class OrderingService : IOrderingService
    {
        public IPurchaseOrderRepository PurchaseOrder { get; }
        public IPurchaseOrderItemRepository Item { get; }
        public IPurchaseOrderCategoryRepository Category { get; }
        public IVendorRepository Vendor { get; }
        public IApproverRepository Approver { get; }
        public IPurchaserRepository Purchaser { get; }
        public ITrackingRepository Tracking { get; }

        public OrderingService(
            IPurchaseOrderRepository purchaseOrder,
            IPurchaseOrderItemRepository item,
            IPurchaseOrderCategoryRepository category,
            IVendorRepository vendor,
            IApproverRepository approver,
            IPurchaserRepository purchaser,
            ITrackingRepository tracking)
        {
            PurchaseOrder = purchaseOrder;
            Item = item;
            Category = category;
            Vendor = vendor;
            Approver = approver;
            Purchaser = purchaser;
            Tracking = tracking;
        }
    }
}
