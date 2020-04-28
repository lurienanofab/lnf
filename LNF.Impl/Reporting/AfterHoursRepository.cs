using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Reporting;
using LNF.Reporting;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Reporting
{
    public class AfterHoursRepository : RepositoryBase, IAfterHoursRepository
    {
        public AfterHoursRepository(ISessionManager mgr) : base(mgr) { }

        public IEnumerable<IAfterHours> GetAfterHours(string name)
        {
            return Session.Query<AfterHours>().Where(x => x.AfterHoursName == name).ToList();
        }
    }
}
