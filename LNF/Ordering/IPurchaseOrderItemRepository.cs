using System.Collections.Generic;

namespace LNF.Ordering
{
    public interface IPurchaseOrderItemRepository
    {
        IPurchaseOrderItem AddItem(string partNum, string description, double unitPrice, int inventoryItemId, int vendorId);
        IPurchaseOrderItem UpdateItem(int itemId, string partNum, string description, double unitPrice, int inventoryItemId);
        IEnumerable<IPurchaseOrderDetail> GetDetails(int itemId);
    }
}
