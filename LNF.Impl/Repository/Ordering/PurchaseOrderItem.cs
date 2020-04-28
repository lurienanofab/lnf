using LNF.DataAccess;
using System.Collections.Generic;

namespace LNF.Impl.Repository.Ordering
{
    public class PurchaseOrderItem : IDataItem
    {
        public PurchaseOrderItem()
        {
            Details = new List<PurchaseOrderDetail>();
        }

        public virtual int ItemID { get; set; }
        public virtual Vendor Vendor { get; set; }
        public virtual string Description { get; set; }
        public virtual string PartNum { get; set; }
        public virtual double UnitPrice { get; set; }
        public virtual bool Active { get; set; }
        public virtual int? InventoryItemID { get; set; }
        public virtual IList<PurchaseOrderDetail> Details { get; set; }
    }
}
