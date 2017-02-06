using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Scheduler;

namespace LNF.Repository.Inventory
{
    public class ResourceInventoryLocation : IDataItem
    {
        public virtual int ResourceInventoryLocationID { get; set; }
        public virtual int InventoryLocationID { get; set; }
        public virtual Resource Resource { get; set; }
    }
}
