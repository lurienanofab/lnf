using LNF.DataAccess;
using LNF.Impl.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LNF.Impl.Data
{
    internal static class ActiveLogExtensions
    {
        private static readonly string[] _allowed = { "Account", "Client", "ClientAccount", "ClientManager", "ClientOrg", "ClientRemote", "Org" };

        internal static IEnumerable<T> FindActive<T>(this IQueryable<T> query, Expression<Func<T, int>> record, DateTime sd, DateTime ed, IQueryable<ActiveLog> alogs) where T : IDataItem
        {
            var list = query.ToList().AsQueryable();

            var join1 = list.Join(alogs, record, i => i.Record, (o, i) => new { Item = o, ActiveLog = i })
                .Where(x => x.ActiveLog.EnableDate < ed && (x.ActiveLog.DisableDate == null || x.ActiveLog.DisableDate.Value > sd))
                .ToList();

            var join2 = join1.Join(
                    join1.GroupBy(x => x.ActiveLog.Record).Select(g => new { Record = g.Key, LogID = g.Max(n => n.ActiveLog.LogID) }),
                    o => o.ActiveLog.LogID,
                    i => i.LogID,
                    (o, i) => o.Item).ToList();

            var result = join1.Select(x => x.Item).ToList();

            return result;
        }

        internal static void Disable(this NHibernate.ISession session, IActiveDataItem item)
        {
            if (session == null)
                throw new ArgumentNullException("session");

            if (item == null)
                throw new ArgumentNullException("item");

            string tableName = item.TableName();

            if (!_allowed.Contains(tableName))
                throw new ArgumentException("TableName is invalid.", "tableName");

            int record = item.Record();

            if (record == 0)
                throw new ArgumentException("Record cannot be zero. If this is a new object it should be saved first.", "record");

            item.Active = false;
            session.Update(item);

            var alog = session.Query<ActiveLog>().FirstOrDefault(x => x.TableName == tableName && x.Record == record && x.DisableDate == null);

            if (alog != null)
            {
                alog.DisableDate = DateTime.Now.Date.AddDays(1);
                session.Update(alog);
            }
        }

        internal static void Enable(this NHibernate.ISession session, IActiveDataItem item)
        {
            if (session == null)
                throw new ArgumentNullException("session");

            if (item == null)
                throw new ArgumentNullException("item");

            string tableName = item.TableName();

            if (!_allowed.Contains(tableName))
                throw new ArgumentException("TableName is invalid.", "tableName");

            int record = item.Record();

            if (record == 0)
                throw new ArgumentException("Record cannot be zero. If this is a new object it should be saved first.", "record");

            item.Active = true;
            session.Update(item);

            var alog = session.Query<ActiveLog>().FirstOrDefault(x => x.TableName == tableName && x.Record == record && x.DisableDate == null);

            // if an ActiveLog with null DisableDate already exists then there is no reason to create a new one or do anything else

            if (alog == null)
            {
                session.Save(new ActiveLog
                {
                    DisableDate = null,
                    EnableDate = DateTime.Now.Date,
                    Record = record,
                    TableName = tableName
                });
            }
        }
    }
}
