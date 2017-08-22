using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Repository.Data
{
    public class Cost : IDataItem
    {
        public virtual int CostID { get; set; }
        public virtual int ChargeTypeID { get; set; }
        public virtual string TableNameOrDescription { get; set; }
        public virtual int RecordID { get; set; }
        public virtual string AcctPer { get; set; }
        public virtual decimal AddVal { get; set; }
        public virtual decimal MulVal { get; set; }
        public virtual DateTime EffDate { get; set; }
        public virtual DateTime CreatedDate { get; set; }

        public virtual ChargeType GetChargeType()
        {
            return DA.Current.Single<ChargeType>(ChargeTypeID);
        }

        private static readonly string[] _toolCostTableNames = { "ToolCost", "ToolOvertimeCost" };

        public static IList<Cost> SelectToolCosts(int resourceId, DateTime cutoff)
        {
            IQueryable<Cost> query;

            if (resourceId == 0)
                query = DA.Current.Query<Cost>().Where(x => _toolCostTableNames.Contains(x.TableNameOrDescription) && x.EffDate < cutoff);
            else
                query = DA.Current.Query<Cost>().Where(x => _toolCostTableNames.Contains(x.TableNameOrDescription) && x.EffDate < cutoff && (x.RecordID == resourceId || x.RecordID == 0));

            var outer = query.ToList();

            var inner = outer
                .GroupBy(x => new { x.ChargeTypeID, x.RecordID, x.TableNameOrDescription })
                .Select(x => new { x.Key.ChargeTypeID, x.Key.RecordID, x.Key.TableNameOrDescription, EffDate = x.Max(g => g.EffDate) })
                .ToList();

            var join = outer.Join(inner,
                o => new { o.ChargeTypeID, o.RecordID, o.TableNameOrDescription, o.EffDate },
                i => new { i.ChargeTypeID, i.RecordID, i.TableNameOrDescription, i.EffDate },
                (o, i) => o).ToList();

            return join;
        }

        public static IList<Cost> SelectToolCosts(DateTime cutoff)
        {
            // all tools
            return SelectToolCosts(0, cutoff);
        }

        public static IList<Cost> SelectToolCosts(int resourceId)
        {
            // most recent
            return SelectToolCosts(resourceId, DateTime.Now);
        }

        public static IList<Cost> SelectToolCosts()
        {
            // most recent
            return SelectToolCosts(DateTime.Now);
        }
    }
}
