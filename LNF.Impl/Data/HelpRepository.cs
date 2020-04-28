using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;

namespace LNF.Impl.Data
{
    public class HelpRepository : RepositoryBase, IHelpRepository
    {
        public HelpRepository(ISessionManager mgr) : base(mgr) { }
    }
}
