using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public static class CostUtility
    {
        public static IList<Cost> FindCosts(string tableNameOrDescription, DateTime cutoff)
        {
            var query = DA.Current.Query<Cost>().Where(x => x.TableNameOrDescription == tableNameOrDescription && x.EffDate < cutoff).ToArray();

            var result = query.Join(
                    query.Where(x => x.TableNameOrDescription == tableNameOrDescription && x.EffDate < cutoff)
                        .GroupBy(x => new { x.ChargeType, x.TableNameOrDescription, x.RecordID })
                        .Select(x => new { x.Key.ChargeType, x.Key.TableNameOrDescription, x.Key.RecordID, EffDate = x.Max(g => g.EffDate) }).ToArray(),
                    o => new { o.ChargeType, o.TableNameOrDescription, o.RecordID, o.EffDate },
                    i => new { i.ChargeType, i.TableNameOrDescription, i.RecordID, i.EffDate },
                    (outer, inner) => outer
                ).OrderBy(x => x.ChargeType.ChargeTypeID)
                .ThenBy(x => x.TableNameOrDescription)
                .ThenBy(x => x.RecordID)
                .ThenBy(x => x.EffDate);

            return result.ToArray();
        }

        public static IList<Cost> FindCosts(string tableNameOrDescription, DateTime chargeDate, int? chargeTypeId = null, int? recordId = null)
        {
            IQueryable<Cost> query;

            if (chargeTypeId.HasValue)
            {
                if (recordId.HasValue)
                    query = DA.Current.Query<Cost>().Where(x => x.TableNameOrDescription == tableNameOrDescription && x.EffDate <= chargeDate && x.ChargeType.ChargeTypeID == chargeTypeId.Value && x.RecordID == recordId.Value);
                else
                    query = DA.Current.Query<Cost>().Where(x => x.TableNameOrDescription == tableNameOrDescription && x.EffDate <= chargeDate && x.ChargeType.ChargeTypeID == chargeTypeId.Value);
            }
            else
            {
                if (recordId.HasValue)
                    query = DA.Current.Query<Cost>().Where(x => x.TableNameOrDescription == tableNameOrDescription && x.EffDate <= chargeDate && x.RecordID == recordId.Value);
                else
                    query = DA.Current.Query<Cost>().Where(x => x.TableNameOrDescription == tableNameOrDescription && x.EffDate <= chargeDate);
            }

            IList<Cost> list = query.ToList();

            //IList<Cost> query = DA.Current.Query<Cost>().Where(x => x.TableNameOrDescription == tableNameOrDescription && x.EffDate <= chargeDate).ToList();

            var agg = list.GroupBy(c => new { c.ChargeType, c.TableNameOrDescription, c.RecordID })
                .Select(g => new { ChargeType = g.Key.ChargeType, TableNameOrDescription = g.Key.TableNameOrDescription, RecordID = g.Key.RecordID, EffDate = g.Max(m => m.EffDate) });

            IList<Cost> result = list.Join(agg, x => new { x.ChargeType, x.TableNameOrDescription, x.RecordID, x.EffDate }, y => new { y.ChargeType, y.TableNameOrDescription, y.RecordID, y.EffDate }, (x, y) => x).ToList();

            //return result.Where(x => x.ChargeType == (chargeTypeId.HasValue ? DA.Current.Single<ChargeType>(chargeTypeId.Value) : x.ChargeType) && x.RecordID == (recordId.HasValue ? recordId : x.RecordID)).ToList();

            return result.ToList();
        }

        public static IList<Cost> FindCostsForToolBilling(DateTime period)
        {
            string[] tableNames = new string[] { "ToolCost", "ToolCreateReservCost", "ToolMissedReservCost", "ToolOvertimeCost" };
            IList<Cost> query = DA.Current.Query<Cost>().Where(x => x.EffDate < period.AddMonths(1) && tableNames.Contains(x.TableNameOrDescription)).ToList();
            var groupBy = query.GroupBy(x => new { x.ChargeType, x.TableNameOrDescription, x.RecordID })
                .Select(g => new { Group = g, Max = g.Max(y => y.CostID) });
            IList<Cost> result = query
                .Where(x => groupBy.Select(a => a.Max).Contains(x.CostID))
                .OrderBy(x => x.ChargeType.ChargeTypeID)
                .ThenBy(x => x.TableNameOrDescription)
                .ThenBy(x => x.RecordID)
                .ThenBy(x => x.EffDate)
                .ToList();
            return result;
        }

        public static IList<Cost> FindAuxiliaryCosts(string tableNameOrDescription, DateTime chargeDate, int? chargeTypeId)
        {
            IList<Cost> query = DA.Current.Query<Cost>().Where(x => x.TableNameOrDescription == tableNameOrDescription && x.EffDate <= chargeDate).ToList();

            var agg = query.GroupBy(c => new { c.ChargeType, c.TableNameOrDescription })
                .Select(g => new { ChargeType = g.Key.ChargeType, TableNameOrDescription = g.Key.TableNameOrDescription, EffDate = g.Max(m => m.EffDate) });

            IList<Cost> result = query.Join(agg, x => new { x.ChargeType, x.TableNameOrDescription, x.EffDate }, y => new { y.ChargeType, y.TableNameOrDescription, y.EffDate }, (x, y) => x).ToList();

            return result.Where(x => x.ChargeType == (chargeTypeId.HasValue ? DA.Current.Single<ChargeType>(chargeTypeId.Value) : x.ChargeType)).ToList();
        }
    }
}
