using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LNF
{
    public class DateRange
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateRange() { }

        public DateRange(DateTime period)
            : this(period, period.AddMonths(1)) { }

        public DateRange(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        protected IActiveLogManager ActiveLogManager => DA.Use<IActiveLogManager>();

        public TimeSpan Span
        {
            get { return EndDate - StartDate; }
        }

        public IEnumerable<ActiveLogItem<T>> Items<T>(Expression<Func<T, ActiveLogKey>> key) where T : class, IActiveDataItem
        {
            return ActiveLogItem<T>.Create(DA.Current.Query<T>(), key, StartDate, EndDate);
        }

        public IEnumerable<ActiveLogItem<T>> Items<T>(IEnumerable<int> records, Expression<Func<T, ActiveLogKey>> key) where T : class, IActiveDataItem
        {
            return ActiveLogItem<T>.Create(DA.Current.Query<T>(), records, key, StartDate, EndDate);
        }

        public IEnumerable<ActiveLogItem<T>> Items<T>(IQueryable<T> query, Expression<Func<T, ActiveLogKey>> key) where T : class, IActiveDataItem
        {
            return ActiveLogItem<T>.Create(query, key, StartDate, EndDate);
        }

        //public IEnumerable<ActiveLogItem<IActiveDataItem>> Items(IEnumerable<IActiveDataItem> items)
        //{
        //    IEnumerable<ActiveLogItem<IActiveDataItem>> result = items.Select(x => ActiveLogItem<IActiveDataItem>.Create(x, StartDate, EndDate, DA.Current));
        //    return result;
        //}

        //public IEnumerable<ActiveLogItem<IActiveDataItem>> Items(string tableName)
        //{
        //    IEnumerable<ActiveLog> query = ActiveLogManager.Range(tableName, StartDate, EndDate);
        //    IEnumerable<ActiveLogItem<IActiveDataItem>> result = query.Select(x => ActiveLogItem<IActiveDataItem>.Create(tableName, x.Record, StartDate, EndDate, DA.Current));
        //    return result;
        //}

        //public IEnumerable<ActiveLogItem<IActiveDataItem>> Items(string tableName, IEnumerable<int> records)
        //{
        //    IEnumerable<ActiveLog> query = ActiveLogManager.Range(tableName, StartDate, EndDate);
        //    IEnumerable<ActiveLogItem<IActiveDataItem>> result = query.Where(x => records.Contains(x.Record)).Select(x => ActiveLogItem<IActiveDataItem>.Create(tableName, x.Record, StartDate, EndDate, DA.Current));
        //    return result;
        //}
    }
}
