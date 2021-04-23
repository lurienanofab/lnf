using LNF.Data;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Data
{
    public class CostRepository : ApiClient, ICostRepository
    {
        public ICost GetCost(int costId)
        {
            return Get<Cost>("webapi/data/cost/{costId}", UrlSegments(new { costId }));
        }

        public IEnumerable<ICost> GetCosts(int limit, int skip = 0)
        {
            return Get<List<Cost>>("webapi/data/cost", QueryStrings(new { limit, skip }));
        }

        public IEnumerable<ICost> FindAuxiliaryCosts(string table, DateTime? cutoff = null, int chargeTypeId = 0)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICost> FindCosts(string[] tables, DateTime? cutoff = null, int recordId = 0, int chargeTypeId = 0)
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

        public IEnumerable<ICost> FindToolCosts(int resourceId, DateTime? cutoff = null, int chargeTypeId = 0)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IGlobalCost> GetGlobalCosts()
        {
            throw new NotImplementedException();
        }

        public IGlobalCost GetActiveGlobalCost()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICost> FindCurrentCosts(string[] tables, int recordId = 0, int chargeTypeId = 0)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICost> FindCurrentToolCosts()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICost> FindCurrentToolCosts(int resourceId, int chargeTypeId = 0)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IChargeType> GetChargeTypes()
        {
            throw new NotImplementedException();
        }
    }
}
