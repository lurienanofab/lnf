using System;

namespace LNF.Store
{
    public interface IPrice
    {
        int PriceID { get; set; }
        int VendorPackageID { get; set; }
        decimal BaseQMultiplier { get; set; }
        decimal PriceBreakQuantity { get; set; }
        decimal PackageCost { get; set; }
        decimal PackageMarkup { get; set; }
        decimal PackagePrice { get; set; }
        DateTime DateActive { get; set; }
    }
}
