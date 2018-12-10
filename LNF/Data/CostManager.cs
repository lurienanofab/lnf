using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public class CostManager : ManagerBase, ICostManager
    {
        public CostManager(ISession session) : base(session) { }

        /// <summary>
        /// Gets the most recent costs.
        /// </summary>
        /// <param name="tables">Filters results by TableNameOrDescription.</param>
        /// <param name="cutoff">Only results with EffDate before (and not including) the cutoff are returned.</param>
        /// <param name="recordId">Only results with the specified RecordID are returned.</param>
        /// <param name="chargeTypeId">Only results with the specified ChargeTypeId are returned.</param>
        public IEnumerable<Cost> FindCosts(string[] tables, DateTime? cutoff = null, int? recordId = null, int? chargeTypeId = null)
        {
            var query = Session.Query<Cost>().Where(x =>
                tables.Contains(x.TableNameOrDescription)
                && (cutoff == null || x.EffDate < cutoff)
                && (chargeTypeId == null || x.ChargeTypeID == chargeTypeId)
                && (recordId == null || x.RecordID == recordId || x.RecordID == null || x.RecordID == 0)).ToList();

            var agg = query.GroupBy(x => new { x.ChargeTypeID, x.TableNameOrDescription, x.RecordID })
                .Select(g => new
                {
                    g.Key.ChargeTypeID,
                    g.Key.TableNameOrDescription,
                    g.Key.RecordID,
                    EffDate = g.Max(m => m.EffDate)
                });

            var result = query.Join(agg,
                    o => new { o.ChargeTypeID, o.TableNameOrDescription, o.RecordID, o.EffDate },
                    i => new { i.ChargeTypeID, i.TableNameOrDescription, i.RecordID, i.EffDate },
                    (o, i) => o)
                .OrderBy(x => x.ChargeTypeID)
                .ThenBy(x => x.TableNameOrDescription)
                .ThenBy(x => x.RecordID)
                .ThenBy(x => x.EffDate);

            return result;
        }

        
        public IEnumerable<Cost> FindCostsForToolBilling(DateTime period)
        {
            string[] tables = new string[] { "ToolCost", "ToolCreateReservCost", "ToolMissedReservCost", "ToolOvertimeCost" };
            return FindCosts(tables, period.AddMonths(1));
        }

        public IEnumerable<Cost> FindToolCosts(DateTime? cutoff)
        {
            string[] tables = new[] { "ToolCost", "ToolOvertimeCost" };
            return FindCosts(tables, cutoff);
        }

        public IEnumerable<Cost> FindToolCosts(int resourceId, DateTime? cutoff)
        {
            string[] tables = new[] { "ToolCost", "ToolOvertimeCost" };
            var result =  FindCosts(tables, cutoff, resourceId).ToList();
            return result;
        }

        public IEnumerable<Cost> FindAuxiliaryCosts(string table, DateTime? cutoff, int? chargeTypeId)
        {
            return FindCosts(new[] { table }, cutoff, chargeTypeId: chargeTypeId);
        }

        public IQueryable<ChargeType> GetChargeTypes()
        {
            return Session.Query<ChargeType>();
        }

        private IEnumerable<CostItem> CreateCostItems(IQueryable<Cost> query)
        {
            var join = query.Join(Session.Query<ChargeType>(),
                o => o.ChargeTypeID,
                i => i.ChargeTypeID,
                (o, i) => new { Cost = o, ChargeType = i });

            return join.Select(x => new CostItem()
            {
                CostID = x.Cost.CostID,
                ChargeTypeID = x.ChargeType.ChargeTypeID,
                ChargeTypeName = x.ChargeType.ChargeTypeName,
                TableNameOrDescription = x.Cost.TableNameOrDescription,
                RecordID = x.Cost.RecordID.GetValueOrDefault(),
                AcctPer = x.Cost.AcctPer,
                AddVal = x.Cost.AddVal,
                MulVal = x.Cost.MulVal,
                EffDate = x.Cost.EffDate,
                CreatedDate = x.Cost.CreatedDate
            }).ToList();
        }
    }
}
