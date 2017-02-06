using LNF.Repository;
using LNF.Repository.Inventory;
using System.Linq;

namespace LNF.Inventory
{
    public static class InventoryLocationUtility
    {
        public static string GetFullLocationName(InventoryLocation loc)
        {
            InventoryLocation[] locs = DA.Current.Query<InventoryLocation>().ToArray();

            string result = loc.LocationName;
            
            InventoryLocation parent = locs.FirstOrDefault(x => x.InventoryLocationID == loc.ParentID);
            
            if (parent != null)
                result = GetFullLocationName(parent) + ":" + result;

            return result;
        }
    }
}
