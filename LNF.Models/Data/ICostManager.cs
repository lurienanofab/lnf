using System;
using System.Collections.Generic;

namespace LNF.Models.Data
{
    public interface ICostManager
    {
        IEnumerable<ICost> GetCosts(int limit, int skip = 0);
        ICost GetCost(int costId);
        IEnumerable<ICost> FindCosts(string[] tables, DateTime? cutoff = null, int? recordId = null, int? chargeTypeId = null);
        IEnumerable<ICost> FindCostsForToolBilling(DateTime period);
        IEnumerable<ICost> FindToolCosts(DateTime? cutoff);
        IEnumerable<ICost> FindToolCosts(int resourceId, DateTime? cutoff = null, int? chargeTypeId = null);
        IEnumerable<ICost> FindAuxiliaryCosts(string table, DateTime? cutoff = null, int? chargeTypeId = null);
    }
}