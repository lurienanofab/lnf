using System;

namespace LNF.Inventory
{
    public interface IItemUpdate
    {
        int ItemUpdateID { get; set; }
        int ItemID { get; set; }
        double BeforeQty { get; set; }
        double UpdateQty { get; set; }
        double AfterQty { get; set; }
        DateTime UpdateDateTime { get; set; }
        string UpdateAction { get; set; }
        int? ItemInventoryLocationID { get; set; }
        int? ClientID { get; set; }
    }
}
