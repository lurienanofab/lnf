using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Scheduler;
using LNF.Scheduler;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Scheduler
{
    public class ActivityRepository : RepositoryBase, IActivityRepository
    {
        public ActivityRepository(ISessionManager mgr) : base(mgr) { }

        public IActivity GetActivity(int activityId)
        {
            return Require<Activity>(activityId);
        }

        public IEnumerable<IActivity> GetActivities()
        {
            return Session.Query<Activity>().ToList();
        }

        public IEnumerable<IActivity> GetActiveActivities()
        {
            return Session.Query<Activity>().Where(x => x.IsActive).ToList();
        }
    }
}
