using LNF.Repository;
using LNF.Repository.Inventory;
using LNF.Repository.Store;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Inventory
{
    public static class ItemUtility
    {
        public static ItemPriceHistory GetItemPriceHistory(this Item item, DateTime endDate)
        {
            //The result can be null if the endDate comes before the earliest DateActive - null means no price set because the item didn't exist yet.
            //Records are already sorted by DateActive DESC in the view

            return DA.Current
                .Query<ItemPriceHistory>()
                .Where(x => x.ItemID == item.ItemID && x.DateActive < endDate)
                .ToArray().LastOrDefault();
        }

        public static IList<Item> MasterList()
        {
            //TODO: Is this used? What is the purpose?
            return DA.Current.Query<Item>().ToList();
        }
    }
}
