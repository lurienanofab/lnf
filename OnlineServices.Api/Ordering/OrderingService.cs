using LNF.Ordering;
using RestSharp;

namespace OnlineServices.Api.Ordering
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

        internal OrderingService(IRestClient rc)
        {
            PurchaseOrder = new PurchaseOrderRepository(rc);
            Item = new PurchaseOrderItemRepository(rc);
            Category = new PurchaseOrderCategoryRepository(rc);
            Vendor = new VendorRepository(rc);
            Approver = new ApproverRepository(rc);
            Purchaser = new PurchaserRepository(rc);
            Tracking = new TrackingRepository(rc);
        }
    }
}