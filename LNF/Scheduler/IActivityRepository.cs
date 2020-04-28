using System.Collections.Generic;

namespace LNF.Scheduler
{
    public interface IActivityRepository
    {
        IEnumerable<IActivity> GetActivities();
        IEnumerable<IActivity> GetActiveActivities();
        IActivity GetActivity(int activityId);
    }
}