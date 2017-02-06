using System;

namespace LNF.Repository.Inventory
{
    public class InventoryItem : IDataItem
    {
        public virtual int ItemID { get; set; }
        public virtual string Description { get; set; }
        public virtual string Notes { get; set; }
        public virtual int CatID { get; set; }
        public virtual string ManufacturerPN { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool StoreDisplay { get; set; }
        public virtual bool CrossCharge { get; set; }
        public virtual int StockQuantity { get; set; }
        public virtual int? MinStockQuantity { get; set; }
        public virtual int StockOnOrder { get; set; }
        public virtual DateTime? OrderDate { get; set; }
        public virtual DateTime? EstimatedArrivalDate { get; set; }
        public virtual string SearchKeyWords { get; set; }
        public virtual bool ItemIsPopular { get; set; }
        public virtual int MaxStockQuantity { get; set; }
        public virtual int ItemInventoryTypeID { get; set; }
        public virtual int CheckOutCategoryID { get; set; }
        public virtual bool IsPopular { get; set; }
        public virtual bool IsCheckOutItem { get; set; }
        public virtual int InventoryTypeID { get; set; }
        public virtual string InventoryTypeName { get; set; }
        public virtual bool InventoryTypeDeleted { get; set; }

        public override bool Equals(object obj)
        {
            InventoryItem item = obj as InventoryItem;
            if (item == null) return false;
            return item.ItemID == ItemID && item.ItemInventoryTypeID == ItemInventoryTypeID;
        }

        public override int GetHashCode()
        {
            return (ItemID.ToString() + "|" + ItemInventoryTypeID.ToString()).GetHashCode();
        }
    }
}
