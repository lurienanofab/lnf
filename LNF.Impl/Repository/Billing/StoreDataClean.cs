using LNF.DataAccess;
using LNF.Impl.Repository.Data;
using LNF.Impl.Repository.Inventory;
using LNF.Impl.Repository.Store;
using System;

namespace LNF.Impl.Repository.Billing
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
