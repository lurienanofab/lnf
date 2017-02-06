using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Inventory
{
    public class InventoryType : IDataItem
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
