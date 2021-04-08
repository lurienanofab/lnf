using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Data
{
    public class CostRepository : RepositoryBase, ICostRepository
    {
        public CostRepository(ISessionManager mgr) : base(mgr) { }

        public ICost GetCost(int costId)
        {
            return Session.Query<Cost>().FirstOrDefault(x => x.CostID == costId);
        }

        public IEnumerable<ICost> GetCosts(int limit, int skip = 0)
        {
            if (limit > 100)
                throw new ArgumentOutOfRangeException("The parameter 'limit' must not be greater than 100.");

            var result = Session.Query<Cost>().Skip(skip).Take(limit).OrderBy(x => x.TableNameOrDescription).ThenBy(x => x.RecordID).ThenBy(x => x.ChargeTypeID).ToList();

            return result;
        }

        /// <summary>
        /// Gets the most recent costs.
        /// </summary>
        /// <param name="tables">Filters results by TableNameOrDescription.</param>
        /// <param name="cutoff">Only results with EffDate before (and not including) the cutoff are returned.</param>
        /// <param name="recordId">Only results with the specified RecordID are returned.</param>
        /// <param name="chargeTypeId">Only results with the specified ChargeTypeId are returned.</param>
        public IEnumerable<ICost> FindCosts(string[] tables, DateTime? cutoff = null, int recordId = 0, int chargeTypeId = 0)
        {
            return Session.FindCosts(tables, cutoff, recordId, chargeTypeId);
        }

        public IEnumerable<ICost> FindCurrentCosts(string[] tables, int recordId = 0, int chargeTypeId = 0)
        {
            return Session.FindCurrentCosts(tables, recordId, chargeTypeId);
        }

        public IEnumerable<ICost> FindCostsForToolBilling(DateTime period)
        {
            string[] tables = new string[] { "ToolCost", "ToolCreateReservCost", "ToolMissedReservCost", "ToolOvertimeCost" };
            return FindCosts(tables, period.AddMonths(1));
        }

        public IEnumerable<ICost> FindToolCosts(DateTime? cutoff)
        {
            string[] tables = new[] { "ToolCost", "ToolOvertimeCost" };
            return FindCosts(tables, cutoff);
        }

        public IEnumerable<ICost> FindCurrentToolCosts()
        {
            string[] tables = new[] { "ToolCost", "ToolOvertimeCost" };
            return FindCurrentCosts(tables);
        }

        public IEnumerable<ICost> FindToolCosts(int resourceId, DateTime? cutoff = null, int chargeTypeId = 0)
        {
            string[] tables = new[] { "ToolCost", "ToolOvertimeCost" };   
            var result = FindCosts(tables, cutoff, resourceId, chargeTypeId);
            return result;
        }

        public IEnumerable<ICost> FindCurrentToolCosts(int resourceId, int chargeTypeId = 0)
        {
            string[] tables = new[] { "ToolCost", "ToolOvertimeCost" };
            var result = FindCurrentCosts(tables, resourceId, chargeTypeId);
            return result;
        }

        public IEnumerable<ICost> FindAuxiliaryCosts(string table, DateTime? cutoff = null, int chargeTypeId = 0)
        {
            return FindCosts(new[] { table }, cutoff, chargeTypeId: chargeTypeId);
        }

        public IGlobalCost GetActiveGlobalCost()
        {
            return Session.SelectGlobalCost().CreateModel<IGlobalCost>();
        }

        public IEnumerable<IGlobalCost> GetGlobalCosts()
        {
            return Session.SelectGlobalCosts().CreateModels<IGlobalCost>();
        }
    }
}