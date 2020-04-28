using System;

namespace LNF.Impl.Repository.Ordering
{
    public class StoreManagerReportItem
    {
        public virtual int ItemID { get; set; }
        public virtual string Description { get; set; }
        public virtual int VendorID { get; set; }
        public virtual string VendorName { get; set; }
        public virtual double UnitPrice { get; set; }
        public virtual string Unit { get; set; }
        public virtual DateTime LastOrdered { get; set; }
        public virtual int StoreItemID { get; set; }
        public virtual string StoreDescription { get; set; }
        public virtual double StorePackagePrice { get; set; }
        public virtual int StorePackageQty { get; set; }
        public virtual double StoreUnitPrice { get; set; }
        public virtual DateTime LastPurchased { get; set; }
    }
}
