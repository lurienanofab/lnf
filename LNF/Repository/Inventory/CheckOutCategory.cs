using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Inventory
{
    public class CheckOutCategory : IDataItem
    {
        public virtual int CheckOutCategoryID { get; set; }
        public virtual InventoryType InventoryType { get; set; }
        public virtual string CategoryName { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
