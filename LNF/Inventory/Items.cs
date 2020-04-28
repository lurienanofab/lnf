using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Inventory
{
    public static class Items
    {
        public static IList<IInventoryItem> All()
        {
            //TODO: Is this used? What is the purpose?
            return ServiceProvider.Current.Inventory.Item.GetItems().ToList();
        }

        public static IItemPriceHistory GetItemPriceHistory(IInventoryItem item, DateTime cutoff)
        {
            // The result can be null if the cutoff comes before the earliest DateActive - null means no price set because the
            // item didn't exist yet. Records are already sorted by DateActive DESC in the view
            return ServiceProvider.Current.Inventory.Item.GetItemPriceHistory(item.ItemID, cutoff);
        }
    }
}
