﻿using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LNF.Data
{
    public static class ActiveLogUtility
    {
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
