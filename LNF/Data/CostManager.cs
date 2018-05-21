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

        public IEnumerable<CostItem> FindCosts(string tableNameOrDescription, DateTime cutoff)
        {
            var query = Session.Query<Cost>().Where(x => x.TableNameOrDescription == tableNameOrDescription && x.EffDate < cutoff);

            var models = CreateCostModels(query);

            var agg = models.GroupBy(x => new { x.ChargeTypeID, x.TableNameOrDescription, x.RecordID })
                .Select(g => new
                {
                    g.Key.ChargeTypeID,
                    TableNameOrDescription = g.Key.TableNameOrDescription,
                    RecordID = g.Key.RecordID,
                    EffDate = g.Max(m => m.EffDate)
                }).ToList();

            var result = models.Join(agg,
                    o => new { o.ChargeTypeID, o.TableNameOrDescription, o.RecordID, o.EffDate },
                    i => new { i.ChargeTypeID, i.TableNameOrDescription, i.RecordID, i.EffDate },
                    (o, i) => o)
                .OrderBy(x => x.ChargeTypeID)
                .ThenBy(x => x.TableNameOrDescription)
                .ThenBy(x => x.RecordID)
                .ThenBy(x => x.EffDate)
                .ToList();

            return result;
        }

        public IEnumerable<CostItem> FindCosts(string tableNameOrDescription, DateTime chargeDate, int? chargeTypeId = null, int? recordId = null)
        {
            IQueryable<Cost> query;

            if (chargeTypeId.HasValue)
            {
                if (recordId.HasValue)
                    query = Session.Query<Cost>().Where(x => x.TableNameOrDescription == tableNameOrDescription && x.EffDate <= chargeDate && x.ChargeTypeID == chargeTypeId.Value && x.RecordID == recordId.Value);
                else
                    query = Session.Query<Cost>().Where(x => x.TableNameOrDescription == tableNameOrDescription && x.EffDate <= chargeDate && x.ChargeTypeID == chargeTypeId.Value);
            }
            else
            {
                if (recordId.HasValue)
                    query = Session.Query<Cost>().Where(x => x.TableNameOrDescription == tableNameOrDescription && x.EffDate <= chargeDate && x.RecordID == recordId.Value);
                else
                    query = Session.Query<Cost>().Where(x => x.TableNameOrDescription == tableNameOrDescription && x.EffDate <= chargeDate);
            }

            var models = CreateCostModels(query);

            var agg = models.GroupBy(c => new { c.ChargeTypeID, c.TableNameOrDescription, c.RecordID })
                .Select(g => new
                {
                    g.Key.ChargeTypeID,
                    TableNameOrDescription = g.Key.TableNameOrDescription,
                    RecordID = g.Key.RecordID,
                    EffDate = g.Max(m => m.EffDate)
                }).ToList();

            var result = models.Join(agg,
                    o => new { o.ChargeTypeID, o.TableNameOrDescription, o.RecordID, o.EffDate },
                    i => new { i.ChargeTypeID, i.TableNameOrDescription, i.RecordID, i.EffDate },
                    (o, i) => o)
                .OrderBy(x => x.ChargeTypeID)
                .ThenBy(x => x.TableNameOrDescription)
                .ThenBy(x => x.RecordID)
                .ThenBy(x => x.EffDate)
                .ToList();

            return result;
        }

        public IEnumerable<Cost> FindCostsForToolBilling(DateTime period)
        {
            string[] tableNames = new string[] { "ToolCost", "ToolCreateReservCost", "ToolMissedReservCost", "ToolOvertimeCost" };

            var query = Session.Query<Cost>().Where(x => x.EffDate < period.AddMonths(1) && tableNames.Contains(x.TableNameOrDescription)).ToList();

            var groupBy = query.GroupBy(x => new { x.ChargeTypeID, x.TableNameOrDescription, x.RecordID })
                .Select(g => new
                {
                    Group = g,
                    Max = g.Max(y => y.CostID)
                });

            IList<Cost> result = query
                .Where(x => groupBy.Select(a => a.Max).Contains(x.CostID))
                .OrderBy(x => x.ChargeTypeID)
                .ThenBy(x => x.TableNameOrDescription)
                .ThenBy(x => x.RecordID)
                .ThenBy(x => x.EffDate)
                .ToList();

            return result;
        }

        public IEnumerable<Cost> FindAuxiliaryCosts(string tableNameOrDescription, DateTime chargeDate, int? chargeTypeId)
        {
            var query = DA.Current.Query<Cost>().Where(x => x.TableNameOrDescription == tableNameOrDescription && x.EffDate <= chargeDate).ToList();

            var agg = query.GroupBy(c => new { c.ChargeTypeID, c.TableNameOrDescription })
                .Select(g => new
                {
                    g.Key.ChargeTypeID,
                    TableNameOrDescription = g.Key.TableNameOrDescription,
                    EffDate = g.Max(m => m.EffDate)
                });

            IList<Cost> result = query.Join(agg,
                o => new { o.ChargeTypeID, o.TableNameOrDescription, o.EffDate },
                i => new { i.ChargeTypeID, i.TableNameOrDescription, i.EffDate },
                (o, i) => o).ToList();

            return result.Where(x => x.ChargeTypeID == chargeTypeId.GetValueOrDefault(x.ChargeTypeID)).ToList();
        }

        private IEnumerable<CostItem> CreateCostModels(IQueryable<Cost> query)
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
                RecordID = x.Cost.RecordID,
                AcctPer = x.Cost.AcctPer,
                AddVal = x.Cost.AddVal,
                MulVal = x.Cost.MulVal,
                EffDate = x.Cost.EffDate,
                CreatedDate = x.Cost.CreatedDate
            }).ToList();
        }
    }
}
