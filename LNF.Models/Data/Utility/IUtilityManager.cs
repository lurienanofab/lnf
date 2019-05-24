using LNF.Models.Data.Utility.BillingChecks;
using System;
using System.Collections.Generic;

namespace LNF.Models.Data.Utility
{
    public interface IUtilityManager
    {
        string GetSiteMenu(int clientId, string target = null);
        string GetSiteMenu(string username, string target = null);
        IEnumerable<AutoEndProblem> GetAutoEndProblems(DateTime period);
        int FixAllAutoEndProblems(DateTime period);
        int FixAutoEndProblem(DateTime period, int reservationId);
    }
}
