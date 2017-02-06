using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public static class ActiveLogUtility
    {
        public static IList<T> FindActive<T>(Func<T, int> record, DateTime sd, DateTime ed) where T : IActiveDataItem
        {
            T entity = Activator.CreateInstance<T>();
            string tableName = entity.TableName();

            //base query
            var baseQuery = DA.Current.Query<ActiveLog>()
                .Where(x => x.TableName == tableName && (x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd)));

            // step1: join to baseQuery
            var step1 = baseQuery.Join(
                DA.Current.Query<T>(),
                o => o.Record,
                record,
                (o, i) => new { ActiveLog = o, Items = i }).ToList();

            // step2: it is possible to have duplicates because of disabling and re-enabling in the same
            //        date range, in this case get the last one by joining to self grouped by max LogID
            var step2 = step1.Join(
                step1.GroupBy(x => x.ActiveLog.Record).Select(g => new { Record = g.Key, LogID = g.Max(n => n.ActiveLog.LogID) }),
                o => o.ActiveLog.LogID,
                i => i.LogID,
                (o, i) => o.Items);

            return step2.ToList();
        }

        /// <summary>
        /// Gets entities in a collection that were active at any point during the date range.
        /// </summary>
        public static IEnumerable<T> Range<T>(IEnumerable<T> items, DateTime startDate, DateTime endDate) where T : IActiveDataItem
        {
            IList<T> result = new List<T>();
            if (items == null || items.Count() == 0) return result;
            string tableName = items.First().TableName();
            IList<ActiveLog> logs = Range(tableName, startDate, endDate, items.Select(x => x.Record()).ToArray());
            return items.Where(x => logs.Select(y => y.Record).Contains(x.Record()));
        }

        public static IList<ActiveLog> Range(string tableName, DateTime startDate, DateTime endDate)
        {
            IList<ActiveLog> query = DA.Current.Query<ActiveLog>().Where(x => x.TableName == tableName && x.EnableDate < endDate && (x.DisableDate == null || x.DisableDate.Value > startDate)).ToList();
            return query;
        }

        public static IList<ActiveLog> Range(string tableName, DateTime startDate, DateTime endDate, IEnumerable<int> records)
        {
            IList<ActiveLog> query = DA.Current.Query<ActiveLog>().Where(x => x.TableName == tableName && records.Contains(x.Record) && x.EnableDate < endDate && (x.DisableDate == null || x.DisableDate.Value > startDate)).ToList();
            return query;
        }

        /// <summary>
        /// Checks to see if an item is currently active
        /// </summary>
        /// <param name="item">The IActiveDataItem item</param>
        /// <returns>True if the item is currently active, otherwise false</returns>
        public static bool IsActive(IActiveDataItem item)
        {
            return DA.Current.Query<ActiveLog>().Any(x => x.TableName == item.TableName() && x.Record == item.Record() && x.DisableDate == null);
        }

        /// <summary>
        /// Checks to see if an item was active during the date range
        /// </summary>
        /// <param name="item">The IActiveDataItem item</param>
        /// <param name="sd">The start of the date range</param>
        /// <param name="ed">The end of the date range</param>
        /// <returns>True if the item was active during the date range, otherwise false</returns>
        public static bool IsActive(IActiveDataItem item, DateTime sd, DateTime ed)
        {
            return DA.Current.Query<ActiveLog>().Any(x => x.TableName == item.TableName() && x.Record == item.Record() && x.EnableDate < ed && (x.DisableDate == null || x.DisableDate.Value > sd));
        }

        /// <summary>
        /// Checks to see if an item is currently active
        /// </summary>
        /// <param name="item">The IActiveLogItem item</param>
        /// <returns>True if the item is currently active, otherwise false</returns>
        public static bool IsActive(IActiveLogItem item)
        {
            return item.DisableDate == null;
        }

        /// <summary>
        /// Checks to see if an item was active during the date range
        /// </summary>
        /// <param name="item">The IActiveLogItem item</param>
        /// <param name="sd">The start of the date range</param>
        /// <param name="ed">The end of the date range</param>
        /// <returns>True if the item was active during the date range, otherwise false</returns>
        public static bool IsActive(IActiveLogItem item, DateTime sd, DateTime ed)
        {
            return item.EnableDate < ed && (item.DisableDate == null || item.DisableDate > sd);
        }
    }
}
