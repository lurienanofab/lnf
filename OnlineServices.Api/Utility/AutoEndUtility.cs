using LNF.Util.AutoEnd;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Utility
{
    public class AutoEndUtility : ApiClient, IAutoEndUtility
    {
        internal AutoEndUtility(IRestClient rc) : base(rc) { }

        public IEnumerable<AutoEndProblem> GetAutoEndProblems(DateTime period)
        {
            var result = Get<List<AutoEndProblem>>("webapi/data/utility/billing-checks/auto-end-problems", QueryStrings(new { period = period.ToString("yyyy-MM-dd") }));
            return result;
        }

        public int FixAllAutoEndProblems(DateTime period)
        {
            var result = Get<int>("webapi/data/utility/billing-checks/auto-end-problems/fix-all", QueryStrings(new { period = period.ToString("yyyy-MM-dd") }));
            return result;
        }

        public int FixAutoEndProblem(DateTime period, int reservationId)
        {
            if (reservationId <= 0)
                throw new ArgumentOutOfRangeException("reservationId");

            var result = Get<int>("webapi/data/utility/billing-checks/auto-end-problems/fix", QueryStrings(new { period = period.ToString("yyyy-MM-dd"), reservationId }));

            return result;
        }
    }
}
