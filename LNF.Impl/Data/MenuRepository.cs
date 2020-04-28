using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Data;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Data
{
    public class MenuRepository : RepositoryBase, IMenuRepository
    {
        public MenuRepository(ISessionManager mgr) : base(mgr) { }

        public IEnumerable<IMenu> GetMenuItems()
        {
            return Session.Query<Menu>()
                .Where(x => x.Active && !x.Deleted)
                .OrderBy(x => x.SortOrder)
                .CreateModels<IMenu>();
        }
    }
}
