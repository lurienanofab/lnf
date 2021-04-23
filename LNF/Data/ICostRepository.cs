using System;
using System.Collections.Generic;

namespace LNF.Data
{
    public interface ICostRepository
    {
        IEnumerable<ICost> GetCosts(int limit, int skip = 0);
        ICost GetCost(int costId);
        IEnumerable<ICost> FindCosts(string[] tables, DateTime? cutoff = null, int recordId = 0, int chargeTypeId = 0);
        IEnumerable<ICost> FindCurrentCosts(string[] tables, int recordId = 0, int chargeTypeId = 0);
        IEnumerable<ICost> FindCostsForToolBilling(DateTime period);
        IEnumerable<ICost> FindToolCosts(DateTime? cutoff);
        IEnumerable<ICost> FindCurrentToolCosts();
        IEnumerable<ICost> FindToolCosts(int resourceId, DateTime? cutoff = null, int chargeTypeId = 0);
        IEnumerable<ICost> FindCurrentToolCosts(int resourceId, int chargeTypeId = 0);
        IEnumerable<ICost> FindAuxiliaryCosts(string table, DateTime? cutoff = null, int chargeTypeId = 0);
        IEnumerable<IGlobalCost> GetGlobalCosts();
        IGlobalCost GetActiveGlobalCost();
        IEnumerable<IChargeType> GetChargeTypes();
    }
}