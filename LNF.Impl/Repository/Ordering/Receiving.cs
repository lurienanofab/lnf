using LNF.DataAccess;
using LNF.Impl.Repository.Data;
using System;

namespace LNF.Impl.Repository.Ordering
{
    public class Receiving : IDataItem
    {
        public virtual int ReceivingID { get; set; }
        public virtual PurchaseOrderDetail PurchaseOrderDetail { get; set; }
        public virtual Client Client { get; set; }
        public virtual double Quantity { get; set; }
        public virtual DateTime ReceivedDate { get; set; }
    }
}
