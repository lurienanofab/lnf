using LNF.DataAccess;
using LNF.Inventory;

namespace LNF.Impl.Repository.Inventory
{
    public class ItemInventoryLocation : IItemInventoryLocation, IDataItem
    {
        public virtual int ItemInventoryLocationID { get; set; }
        public virtual int InventoryLocationID { get; set; }
        public virtual int ItemID { get; set; }
    }
}
