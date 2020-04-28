using System;

namespace LNF.Store
{
    public class PriceItem : IPrice
    {
        public int PriceID { get; set; }
        public int VendorPackageID { get; set; }
        public decimal PriceBreakQuantity { get; set; }
        public decimal PackageCost { get; set; }
        public decimal PackageMarkup { get; set; }
        public decimal PackagePrice { get; set; }
        public DateTime DateActive { get; set; }
        public decimal BaseQMultiplier { get; set; }
    }
}
