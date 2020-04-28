using LNF.Cache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Inventory
{
    public static class InventoryLocations
    {
        public static IEnumerable<IInventoryLocation> All() => CacheManager.Current.GetValue("InventoryLocations", p => p.Inventory.Item.GetInventoryLocations(), DateTimeOffset.Now.AddDays(1));

        public static string GetFullLocationName(IInventoryLocation loc)
        {
            string result = loc.LocationName;

            var parent = All().FirstOrDefault(x => x.InventoryLocationID == loc.ParentID);
            
            if (parent != null)
                result = GetFullLocationName(parent) + ":" + result;

            return result;
        }
    }
}
