using LNF.Ordering;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Ordering
{
    public class PurchaseOrderItemRepository : ApiClient, IPurchaseOrderItemRepository
    {
        public IPurchaseOrderItem AddItem(string partNum, string description, double unitPrice, int inventoryItemId, int vendorId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPurchaseOrderDetail> GetDetails(int itemId)
        {
            throw new NotImplementedException();
        }

        public IPurchaseOrderItem UpdateItem(int itemId, string partNum, string description, double unitPrice, int inventoryItemId)
        {
            throw new NotImplementedException();
        }
    }
}
