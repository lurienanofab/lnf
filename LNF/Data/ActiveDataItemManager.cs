using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LNF.Data
{
    public class ActiveDataItemManager : ManagerBase, IActiveDataItemManager
    {
        public ActiveDataItemManager(ISession session) : base(session) { }

        /// <summary>
        /// Sets Active to false and updates ActiveLog
        /// </summary>
        public void Disable(IActiveDataItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            int record = item.Record();
            if (record == 0)
                throw new ArgumentException("Record cannot be zero. If this is a new object it should be saved first.", "item");

            item.Active = false;

            ActiveLog alog = Session.Query<ActiveLog>().FirstOrDefault(x => x.Record == record && x.TableName == item.TableName() && x.DisableDate == null);

            // if an ActiveLog with null DisableDate does not already exist then there is no reason to do anything else
            if (alog != null)
            {
                alog.DisableDate = DateTime.Now.Date.AddDays(1);
            }
        }

        /// <summary>
        /// Sets Active to true and updates ActiveLog
        /// </summary>
        public void Enable(IActiveDataItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            int record = item.Record();

            if (record == 0)
                throw new ArgumentException("Record cannot be zero. If this is a new object it should be inserted first.", "item");

            item.Active = true;

            ActiveLog alog = Session.Query<ActiveLog>().FirstOrDefault(x => x.Record == record && x.TableName == item.TableName() && !x.DisableDate.HasValue);

            // if an ActiveLog with null DisableDate already exists then there is no reason to create a new one or do anything else
            if (alog == null)
            {
                Session.Insert(new ActiveLog()
                {
                    DisableDate = null,
                    EnableDate = DateTime.Now.Date,
                    Record = record,
                    TableName = item.TableName()
                });
            }
        }

        public IEnumerable<T> FindActive<T>(IQueryable<T> query, Expression<Func<T, int>> record, DateTime sd, DateTime ed) where T : IActiveDataItem
        {
            string tableName = Activator.CreateInstance<T>().TableName();

            var alogs = Session.Query<ActiveLog>();

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

        /// <summary>
        /// Gets all ActiveLog entities for the IActiveDataItem item.
        /// </summary>
        public IQueryable<ActiveLog> ActiveLogs(IActiveDataItem item)
        {
            return Session.Query<ActiveLog>().Where(x => x.TableName == item.TableName() && x.Record == item.Record());
        }

        /// <summary>
        /// Gets a collection of ActiveLogItems that were active during the date range and have a matching record in the collection of IActiveDataItems.
        /// </summary>
        public IEnumerable<ActiveLogItem<IActiveDataItem>> Range(IEnumerable<IActiveDataItem> list, DateTime startDate, DateTime endDate)
        {
            DateRange range = new DateRange(startDate, endDate);
            IEnumerable<int> records = list.Select(x => x.Record());
            IEnumerable<ActiveLogItem<IActiveDataItem>> result = range.Items(list);
            return result;
        }
    }

}
