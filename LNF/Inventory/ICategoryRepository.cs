using System.Collections.Generic;

namespace LNF.Inventory
{
    public interface ICategoryRepository
    {
        ICategory GetCategory(int catId);
        IEnumerable<ICategory> GetCategories();
    }
}
