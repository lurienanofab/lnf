using LNF.Repository.Inventory;
using System;

namespace LNF.Repository.Data
{
    public class StoreDataClean : IDataItem
    {
        public virtual int StoreDataID { get; set; }
        public virtual Client Client { get; set; }
        public virtual Item Item { get; set; }
        public virtual DateTime OrderDate { get; set; }
        public virtual Account Account { get; set; }
        public virtual decimal Quantity { get; set; }
        public virtual decimal UnitCost { get; set; }
        public virtual Category Category { get; set; }
        public virtual bool RechargeItem { get; set; }
        public virtual DateTime StatusChangeDate { get; set; }
    }
}
