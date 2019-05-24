using LNF.Models.Data;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Data
{
    public class CostManager : ApiClient, ICostManager
    {
        public ICost GetCost(int costId)
        {
            return Get<CostItem>("webapi/data/cost/{costId}", UrlSegments(new { costId }));
        }

        public IEnumerable<ICost> GetCosts(int limit, int skip = 0)
        {
            return Get<List<CostItem>>("webapi/data/cost", QueryStrings(new { limit, skip }));
        }

        public IEnumerable<ICost> FindAuxiliaryCosts(string table, DateTime? cutoff = null, int? chargeTypeId = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICost> FindCosts(string[] tables, DateTime? cutoff = null, int? recordId = null, int? chargeTypeId = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICost> FindCostsForToolBilling(DateTime period)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICost> FindToolCosts(DateTime? cutoff)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICost> FindToolCosts(int resourceId, DateTime? cutoff = null, int? chargeTypeId = null)
        {
            throw new NotImplementedException();
        }
    }
}
