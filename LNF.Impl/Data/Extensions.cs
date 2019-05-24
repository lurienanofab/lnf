using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LNF.Impl.Data
{
    internal static class Extensions
    {
        internal static IEnumerable<T> FindActive<T>(this IQueryable<T> query, Expression<Func<T, int>> record, DateTime sd, DateTime ed, string tableName = null) where T : IDataItem
        {
            if (string.IsNullOrEmpty(tableName))
                tableName = typeof(T).Name;

            var alogs = DA.Current.Query<ActiveLog>();

            var join1 = query.Join(alogs, record, i => i.Record, (o, i) => new { Item = o, ActiveLog = i })
                .Where(x => x.ActiveLog.TableName == tableName && x.ActiveLog.EnableDate < ed && (x.ActiveLog.DisableDate == null || x.ActiveLog.DisableDate.Value > sd))
                .ToList();

            var join2 = join1.Join(
                    join1.GroupBy(x => x.ActiveLog.Record).Select(g => new { Record = g.Key, LogID = g.Max(n => n.ActiveLog.LogID) }),
                    o => o.ActiveLog.LogID,
                    i => i.LogID,
                    (o, i) => o.Item);

            return join1.Select(x => x.Item);
        }
    }
}
