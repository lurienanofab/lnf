using System;

namespace LNF.Ordering
{
    public interface IPurchaseOrderDetail
    {
        int PODID { get; set; }
        int POID { get; set; }
        int ItemID { get; set; }
        int CategoryID { get; set; }
        double Quantity { get; set; }
        double UnitPrice { get; set; }
        string Unit { get; set; }
        DateTime? ToInventoryDate { get; set; }
        bool? IsInventoryControlled { get; set; }
        OrderStatus StatusID { get; set; }
        string Notes { get; set; }
        string PartNum { get; set; }
        int CatID { get; set; }
        string CatName { get; set; }
        int ParentID { get; set; }
        string CatNo { get; set; }
        DateTime CreatedDate { get; set; }
        int? InventoryItemID { get; set; }
    }
}
