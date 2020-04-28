using LNF.Reporting;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Reporting
{
    public class AfterHoursRepository : ApiClient, IAfterHoursRepository
    {
        public IEnumerable<IAfterHours> GetAfterHours(string name)
        {
            throw new NotImplementedException();
        }
    }
}
