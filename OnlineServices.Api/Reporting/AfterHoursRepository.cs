using LNF.Reporting;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Reporting
{
    public class AfterHoursRepository : ApiClient, IAfterHoursRepository
    {
        internal AfterHoursRepository(IRestClient rc) : base(rc) { }

        public IEnumerable<IAfterHours> GetAfterHours(string name)
        {
            throw new NotImplementedException();
        }
    }
}
