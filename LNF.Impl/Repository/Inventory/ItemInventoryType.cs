using LNF.DataAccess;
using LNF.Inventory;

namespace LNF.Impl.Repository.Inventory
{
    public class ItemInventoryType : IItemInventoryType, IDataItem
    {
        public virtual int ItemInventoryTypeID { get; set; }
        public virtual int ItemID { get; set; }
        public virtual int InventoryTypeID { get; set; }
        public virtual int CheckOutCategoryID { get; set; }
        public virtual bool IsPopular { get; set; }
        public virtual bool IsCheckOutItem { get; set; }
    }
}
