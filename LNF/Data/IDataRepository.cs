using LNF.Models.Data;
using System;
using System.Collections.Generic;

namespace LNF.Data
{
    /// <summary>
    /// This interface is for requirements that cannot be met by LNF.Repository.ISession data interactions.
    /// </summary>
    public interface IDataRepository
    {
        IEnumerable<CostModel> FindCosts(string tableNameOrDescription, DateTime chargeDate, int? chargeTypeId = null, int? recordId = null);
    }
}
