using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Billing
{
    public class StoreData : IDataItem
    {
        public virtual int StoreDataID { get; set; }
        public virtual DateTime Period { get; set; }
        public virtual int ClientID { get; set; }
        public virtual int ItemID { get; set; }
        public virtual DateTime OrderDate { get; set; }
        public virtual int AccountID { get; set; }
        public virtual double Quantity { get; set; }
        public virtual double UnitCost { get; set; }
        public virtual int CategoryID { get; set; }
        public virtual DateTime StatusChangeDate { get; set; }
    }
}
