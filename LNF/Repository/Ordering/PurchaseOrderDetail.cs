using System;

namespace LNF.Repository.Ordering
{
    public class PurchaseOrderDetail : IDataItem
    {
        public virtual int PODID { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public virtual PurchaseOrderItem Item { get; set; }
        public virtual PurchaseOrderCategory Category { get; set; }
        public virtual double Quantity { get; set; }
        public virtual double UnitPrice { get; set; }
        public virtual string Unit { get; set; }
        public virtual DateTime? ToInventoryDate { get; set; }
        public virtual bool? IsInventoryControlled { get; set; }
    }
}
