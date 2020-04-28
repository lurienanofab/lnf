using LNF.Scheduler;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Scheduler
{
    public class ActivityRepository : ApiClient, IActivityRepository
    {
        public IEnumerable<IActivity> GetActiveActivities()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IActivity> GetActivities()
        {
            throw new NotImplementedException();
        }

        public IActivity GetActivity(int activityId)
        {
            throw new NotImplementedException();
        }
    }
}
