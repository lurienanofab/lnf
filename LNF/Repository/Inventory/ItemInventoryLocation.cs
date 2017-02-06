using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Inventory
{
    public class ItemInventoryLocation : IDataItem
    {
        public virtual int ItemInventoryLocationID { get; set; }
        public virtual InventoryLocation InventoryLocation { get; set; }
        public virtual Item Item { get; set; }
    }
}
