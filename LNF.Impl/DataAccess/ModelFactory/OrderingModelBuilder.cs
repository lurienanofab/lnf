using LNF.Impl.Repository.Data;
using LNF.Impl.Repository.Ordering;
using LNF.Ordering;

namespace LNF.Impl.DataAccess.ModelFactory
{
    public class OrderingModelBuilder : ModelBuilder
    {
        public OrderingModelBuilder(ISessionManager mgr) : base(mgr) { }

        private IApprover CreateApproverModel(Approver source)
        {
            var approver = Session.Get<ClientInfo>(source.ApproverID);
            var client = Session.Get<ClientInfo>(source.ClientID);

            var result = MapFrom<ApproverItem>(source);
            result.ApproverDisplayName = approver != null ? approver.DisplayName : string.Empty;
            result.DisplayName = client != null ? client.DisplayName : string.Empty;
            return result;
        }

        private IPurchaseOrder CreatePurchaseOrderModel(PurchaseOrder source)
        {
            int clientId = 0;
            int approverId = 0;
            int shippingMethodId = 0;
            int statusId = 0;
            int vendorId = 0;

            if (source.Client != null)
                clientId = source.Client.ClientID;

            if (source.Approver != null)
                approverId = source.Approver.ClientID;

            if (source.ShippingMethod != null)
                shippingMethodId = source.ShippingMethod.ShippingMethodID;

            if (source.Status != null)
                statusId = source.Status.StatusID;

            if (source.Vendor != null)
                vendorId = source.Vendor.VendorID;

            var result = new LNF.Ordering.PurchaseOrderItem
            {
                AccountID = source.AccountID,
                ApprovalDate = source.ApprovalDate,
                ApproverID = approverId,
                Attention = source.Attention,
                ClientID = clientId,
                CompletedDate = source.CompletedDate,
                CreatedDate = source.CreatedDate,
                NeededDate = source.NeededDate,
                Notes = source.Notes,
                Oversized = source.Oversized,
                POID = source.POID,
                PurchaserID = source.PurchaserID,
                PurchaserNotes = source.PurchaserNotes,
                RealApproverID = source.RealApproverID,
                RealPO = source.RealPO,
                ReqNum = source.ReqNum,
                ShippingMethodID = shippingMethodId,
                StatusID = statusId,
                VendorID = vendorId
            };

            return result;
        }

        public override void AddMaps()
        {
            Map<Approver, IApprover>(x => CreateApproverModel(x));
            Map<PurchaseOrder, IPurchaseOrder>(x => CreatePurchaseOrderModel(x));
        }
    }
}
