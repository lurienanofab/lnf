using System;

namespace LNF.Ordering
{
    public class PurchaseOrderDetailItem : IPurchaseOrderDetail
    {
        public int PODID { get; set; }
        public int POID { get; set; }
        public int ItemID { get; set; }
        public int CategoryID { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public string Unit { get; set; }
        public DateTime? ToInventoryDate { get; set; }
        public bool? IsInventoryControlled { get; set; }
        public OrderStatus StatusID { get; set; }
        public string Notes { get; set; }
        public string PartNum { get; set; }
        public int CatID { get; set; }
        public string CatName { get; set; }
        public int ParentID { get; set; }
        public string CatNo { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? InventoryItemID { get; set; }
    }
}
