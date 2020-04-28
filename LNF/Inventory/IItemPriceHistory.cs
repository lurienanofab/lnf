using System;

namespace LNF.Inventory
{
    public interface IItemPriceHistory
    {
        int ItemID { get; set; }
        int PackageID { get; set; }
        int VendorPackageID { get; set; }
        int PriceID { get; set; }
        DateTime DateActive { get; set; }
        int BaseQMultiplier { get; set; }
        double PackageCost { get; set; }
        double PackageMarkup { get; set; }
        double PackagePrice { get; set; }
        double UnitCost { get; set; }
        double UnitPrice { get; set; }
    }
}
