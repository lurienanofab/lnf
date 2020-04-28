using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Inventory;
using LNF.Inventory;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Inventory
{
    public class CategoryRepository : RepositoryBase, ICategoryRepository
    {
        public CategoryRepository(ISessionManager mgr) : base(mgr) { }

        public ICategory GetCategory(int catId)
        {
            return Require<Category>(catId);
        }

        public IEnumerable<ICategory> GetCategories()
        {
            return Session.Query<Category>().ToList();
        }
    }
}
