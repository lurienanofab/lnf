using System;
using System.Collections.Generic;

namespace LNF.Repository.Inventory
{
    public class Item : IDataItem
    {
        public Item()
        {
            ItemInventoryTypes = new List<ItemInventoryType>();
            ItemInventoryLocations = new List<ItemInventoryLocation>();
            ItemUpdates = new List<ItemUpdate>();
        }

        public virtual int ItemID { get; set; }
        public virtual string Description { get; set; }
        public virtual string Notes { get; set; }
        public virtual Category Category { get; set; }
        public virtual string ManufacturerPN { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool StoreDisplay { get; set; }
        public virtual bool CrossCharge { get; set; }
        public virtual int StockQuantity { get; set; }
        public virtual int? MinStockQuantity { get; set; }
        public virtual int? MaxStockQuantity { get; set; }
        public virtual int StockOnOrder { get; set; }
        public virtual DateTime? OrderDate { get; set; }
        public virtual DateTime? EstimatedArrivalDate { get; set; }
        public virtual string SearchKeyWords { get; set; }
        public virtual bool IsChemicalInventory { get; set; }
        public virtual bool IsPopular { get; set; }
        public virtual IList<ItemInventoryType> ItemInventoryTypes { get; set; }
        public virtual IList<ItemInventoryLocation> ItemInventoryLocations { get; set; }
        public virtual IList<ItemUpdate> ItemUpdates { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Item)
            {
                var i = (Item)obj;
                return i.ItemID == ItemID;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ItemID.GetHashCode();
        }
    }
}
