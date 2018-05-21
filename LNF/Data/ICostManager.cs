using System;
using System.Collections.Generic;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Data
{
    public interface ICostManager : IManager
    {
        IEnumerable<Cost> FindAuxiliaryCosts(string tableNameOrDescription, DateTime chargeDate, int? chargeTypeId);
        IEnumerable<CostItem> FindCosts(string tableNameOrDescription, DateTime cutoff);
        IEnumerable<CostItem> FindCosts(string tableNameOrDescription, DateTime chargeDate, int? chargeTypeId = null, int? recordId = null);
        IEnumerable<Cost> FindCostsForToolBilling(DateTime period);
    }
}