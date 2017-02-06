using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Inventory
{
    public class ItemInventoryType : IDataItem
    {
        public virtual int ItemInventoryTypeID { get; set; }
        public virtual Item Item { get; set; }
        public virtual InventoryType InventoryType { get; set; }
        public virtual CheckOutCategory CheckOutCategory { get; set; }
        public virtual bool IsPopular { get; set; }
        public virtual bool IsCheckOutItem { get; set; }
    }
}
