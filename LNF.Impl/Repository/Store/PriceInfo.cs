using LNF.DataAccess;
using LNF.Store;
using System;

namespace LNF.Impl.Repository.Store
{
    public class PriceInfo : IPrice, IDataItem
    {
        public virtual int PriceID { get; set; }
        public virtual int VendorPackageID { get; set; }
        public virtual int ItemID { get; set; }
        public virtual decimal BaseQMultiplier { get; set; }
        public virtual decimal PriceBreakQuantity { get; set; }
        public virtual decimal PackageCost { get; set; }
        public virtual decimal PackageMarkup { get; set; }
        public virtual decimal PackagePrice { get; set; }
        public virtual DateTime DateActive { get; set; }
    }
}
