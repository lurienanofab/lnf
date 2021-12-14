using LNF.Scheduler;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Scheduler
{
    public class SchedulerPropertyRepository : ApiClient, ISchedulerPropertyRepository
    {
        internal SchedulerPropertyRepository(IRestClient rc) : base(rc) { }

        public IEnumerable<ISchedulerProperty> GetSchedulerProperties()
        {
            throw new NotImplementedException();
        }
    }
}
