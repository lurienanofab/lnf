using LNF.DataAccess;
using LNF.Inventory;

namespace LNF.Impl.Repository.Inventory
{
    public class InventoryLocation : IInventoryLocation, IDataItem
    {
        public virtual int InventoryLocationID { get; set; }
        public virtual int ParentID { get; set; }
        public virtual string LocationName { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool IsStoreLocation { get; set; }
        public virtual string LocationType { get; set; }
    }
}
