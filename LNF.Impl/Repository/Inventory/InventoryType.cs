using LNF.DataAccess;
using LNF.Inventory;
using System.Collections.Generic;

namespace LNF.Impl.Repository.Inventory
{
    public class InventoryType : IInventoryType, IDataItem
    {
        public InventoryType()
        {
            ItemInventoryTypes = new List<ItemInventoryType>();
            CheckOutCategories = new List<CheckOutCategory>();
        }

        public virtual int InventoryTypeID { get; set; }
        public virtual string InventoryTypeName { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual IList<ItemInventoryType> ItemInventoryTypes { get; set; }
        public virtual IList<CheckOutCategory> CheckOutCategories { get; set; }
    }
}
