using LNF.DataAccess;
using LNF.Inventory;
using System;

namespace LNF.Impl.Repository.Inventory
{
    public class ItemUpdate : IItemUpdate, IDataItem
    {
        public virtual int ItemUpdateID { get; set; }
        public virtual int ItemID { get; set; }
        public virtual double BeforeQty { get; set; }
        public virtual double UpdateQty { get; set; }
        public virtual double AfterQty { get; set; }
        public virtual DateTime UpdateDateTime { get; set; }
        public virtual string UpdateAction { get; set; }
        public virtual int? ItemInventoryLocationID { get; set; }
        public virtual int? ClientID { get; set; }
    }
}
