using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Inventory
{
    public class ItemUpdate : IDataItem
    {
        public virtual int ItemUpdateID { get; set; }
        public virtual Item Item { get; set; }
        public virtual double BeforeQty { get; set; }
        public virtual double UpdateQty { get; set; }
        public virtual double AfterQty { get; set; }
        public virtual DateTime UpdateDateTime { get; set; }
        public virtual string UpdateAction { get; set; }
        public virtual int? ItemInventoryLocationID { get; set; }
        public virtual int? ClientID { get; set; }
    }
}
