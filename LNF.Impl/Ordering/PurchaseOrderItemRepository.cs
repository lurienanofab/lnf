using LNF.CommonTools;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Ordering;
using LNF.Ordering;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Ordering
{
    public class PurchaseOrderItemRepository : RepositoryBase, IPurchaseOrderItemRepository
    {
        public PurchaseOrderItemRepository(ISessionManager mgr) : base(mgr) { }

        public IEnumerable<IPurchaseOrderDetail> GetDetails(int itemId)
        {
            return Session.Query<PurchaseOrderDetail>().Where(x => x.Item.ItemID == itemId).CreateModels<IPurchaseOrderDetail>();
        }

        public IPurchaseOrderItem AddItem(string partNum, string description, double unitPrice, int inventoryItemId, int vendorId)
        {
            var item = new Repository.Ordering.PurchaseOrderItem()
            {
                Active = true,
                Description = description,
                InventoryItemID = Utility.ConvertToNullableInt32(inventoryItemId),
                PartNum = partNum,
                UnitPrice = unitPrice,
                Vendor = Session.Get<Vendor>(vendorId)
            };

            Session.Save(item);

            return item.CreateModel<IPurchaseOrderItem>();
        }

        public IPurchaseOrderItem UpdateItem(int itemId, string partNum, string description, double unitPrice, int inventoryItemId)
        {
            var item = Session.Get<Repository.Ordering.PurchaseOrderItem>(itemId);

            if (item == null)
                throw new ItemNotFoundException<Repository.Ordering.PurchaseOrderItem>(x => x.ItemID, itemId);

            item.Description = description;
            item.PartNum = partNum;
            item.UnitPrice = unitPrice;
            item.InventoryItemID = Utility.ConvertToNullableInt32(inventoryItemId);

            Session.Update(item);

            return item.CreateModel<IPurchaseOrderItem>();
        }
    }
}
