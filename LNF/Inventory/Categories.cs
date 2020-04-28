using LNF.Cache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Inventory
{
    public static class Categories
    {
        public static IEnumerable<ICategory> All() => CacheManager.Current.GetValue("Categories", p => p.Inventory.Category.GetCategories(), DateTimeOffset.Now.AddDays(7));

        public static IEnumerable<ICategory> Parents()
        {
            return All().Where(x => x.CatID == x.ParentID);
        }
    }
}
