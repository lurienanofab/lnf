using LNF.Models.Data.Utility;
using LNF.Models.Data.Utility.BillingChecks;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Data
{
    public class UtilityManager : ApiClient, IUtilityManager
    {
        public string GetSiteMenu(int clientId, string target = null)
        {
            if (clientId <= 0)
                throw new ArgumentOutOfRangeException("clientId");

            var result = Get("webapi/data/ajax/menu", QueryStrings(new { clientId, target }));

            return result;
        }

        public string GetSiteMenu(string username, string target = null)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("usernmae");

            var result = Get("webapi/data/ajax/menu", QueryStrings(new { username, target }));

            return result;
        }

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
