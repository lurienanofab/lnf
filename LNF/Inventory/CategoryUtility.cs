using LNF.Repository;
using LNF.Repository.Inventory;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Inventory
{
    public static class CategoryUtility
    {
        public static IList<Category> Parents()
        {
            return DA.Current.Query<Category>().Where(x => x.CatID == x.Parent.CatID).ToList();
        }
    }
}
