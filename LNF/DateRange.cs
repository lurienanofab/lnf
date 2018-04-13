using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;

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

        public TimeSpan Span
        {
            get { return EndDate - StartDate; }
        }

        public IEnumerable<ActiveLogItem<T>> Items<T>() where T : class, IActiveDataItem
        {
            T temp = Activator.CreateInstance<T>();
            ActiveLog[] query = DA.Current.Query<ActiveLog>().Where(x => x.TableName == temp.TableName() && x.EnableDate < EndDate && (x.DisableDate == null || x.DisableDate > StartDate)).ToArray();
            IEnumerable<ActiveLogItem<T>> result = query.Select(x => ActiveLogItem<T>.Create(DA.Current.Single<T>(x.Record), StartDate, EndDate, DA.Current));
            return result;
        }

        public IEnumerable<ActiveLogItem<T>> Items<T>(IEnumerable<int> records) where T : class, IActiveDataItem
        {
            IEnumerable<ActiveLogItem<T>> query = Items<T>();
            IEnumerable<ActiveLogItem<T>> result = query.Where(x => records.Contains(x.Item.Record()));
            return result;
        }

        public IEnumerable<ActiveLogItem<IActiveDataItem>> Items(IEnumerable<IActiveDataItem> items)
        {
            IEnumerable<ActiveLogItem<IActiveDataItem>> result = items.Select(x => ActiveLogItem<IActiveDataItem>.Create(x, StartDate, EndDate, DA.Current));
            return result;
        }

        public IEnumerable<ActiveLogItem<IActiveDataItem>> Items(string tableName)
        {
            IEnumerable<ActiveLog> query = DA.Current.ActiveLogManager().Range(tableName, StartDate, EndDate);
            IEnumerable<ActiveLogItem<IActiveDataItem>> result = query.Select(x => ActiveLogItem<IActiveDataItem>.Create(tableName, x.Record, StartDate, EndDate, DA.Current));
            return result;
        }

        public IEnumerable<ActiveLogItem<IActiveDataItem>> Items(string tableName, IEnumerable<int> records)
        {
            IEnumerable<ActiveLog> query = DA.Current.ActiveLogManager().Range(tableName, StartDate, EndDate);
            IEnumerable<ActiveLogItem<IActiveDataItem>> result = query.Where(x => records.Contains(x.Record)).Select(x => ActiveLogItem<IActiveDataItem>.Create(tableName, x.Record, StartDate, EndDate, DA.Current));
            return result;
        }
    }
}
