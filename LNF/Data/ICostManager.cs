using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public interface ICostManager : IManager
    {
        IEnumerable<Cost> FindCosts(string[] tables, DateTime? cutoff = null, int? recordId = null, int? chargeTypeId = null);
        IEnumerable<Cost> FindCostsForToolBilling(DateTime period);
        IEnumerable<Cost> FindToolCosts(DateTime? cutoff);
        IEnumerable<Cost> FindToolCosts(int resourceId, DateTime? cutoff);
        IEnumerable<Cost> FindAuxiliaryCosts(string table, DateTime? cutoff, int? chargeTypeId);
        IQueryable<ChargeType> GetChargeTypes();
    }
}