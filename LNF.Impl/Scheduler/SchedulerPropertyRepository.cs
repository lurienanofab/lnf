using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Scheduler;
using LNF.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LNF.Impl.Scheduler
{
    public class SchedulerPropertyRepository : RepositoryBase, ISchedulerPropertyRepository
    {
        public SchedulerPropertyRepository(ISessionManager mgr) : base(mgr) { }

        public IEnumerable<ISchedulerProperty> GetSchedulerProperties()
        {
            return Session.Query<SchedulerProperty>().ToList();
        }
    }
}
