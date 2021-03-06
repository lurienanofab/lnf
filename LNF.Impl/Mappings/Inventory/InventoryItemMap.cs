﻿using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Inventory;

namespace LNF.Impl.Mappings.Inventory
{
    internal class InventoryItemMap : ClassMap<InventoryItem>
    {
        internal InventoryItemMap()
        {
            Schema("InventoryControl.dbo");
            Table("v_InventoryItems");
            ReadOnly();
            CompositeId()
                .KeyProperty(x => x.ItemID)
                .KeyProperty(x => x.ItemInventoryTypeID);
            Map(x => x.Description);
            Map(x => x.Notes);
            Map(x => x.CatID);
            Map(x => x.ManufacturerPN);
            Map(x => x.Active);
            Map(x => x.StoreDisplay);
            Map(x => x.CrossCharge);
            Map(x => x.StockQuantity);
            Map(x => x.MinStockQuantity);
            Map(x => x.MaxStockQuantity);
            Map(x => x.StockOnOrder);
            Map(x => x.OrderDate);
            Map(x => x.EstimatedArrivalDate);
            Map(x => x.SearchKeyWords);
            Map(x => x.IsChemicalInventory);
            Map(x => x.IsPopular);
            Map(x => x.CheckOutCategoryID);
            Map(x => x.IsCheckOutItem);
            Map(x => x.InventoryTypeID);
            Map(x => x.InventoryTypeName);
            Map(x => x.InventoryTypeDeleted);
        }
    }
}
