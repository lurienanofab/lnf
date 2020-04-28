using LNF.Scheduler;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Scheduler
{
    public class SchedulerPropertyRepository : ApiClient, ISchedulerPropertyRepository
    {
        public IEnumerable<ISchedulerProperty> GetSchedulerProperties()
        {
            throw new NotImplementedException();
        }
    }
}
