using LNF.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LNF.Impl.Repository.Data
{
    public enum ActiveLogType
    {
        Account = 1,
        Client = 2,
        ClientAccount = 3,
        ClientManager = 4,
        ClientOrg = 5,
        ClientRemote = 6,
        Org = 7
    }

    public class ActiveLogItem<T> where T : IActiveDataItem
    {
        public T Item { get; set; }
        public ActiveLog ActiveLog { get; set; }

        public ActiveLogItem() { }

        public ActiveLogItem(T item, ActiveLog alog)
        {
            Item = item;
            ActiveLog = alog;
        }

        public static ActiveLogItem<T> Create(ISession session, string tableName, int record, DateTime sd, DateTime ed)
        {
            return Create(session, GetActiveLogType(tableName), record, sd, ed);
        }

        public static ActiveLogItem<T> Create(ISession session, ActiveLogType type, int record, DateTime sd, DateTime ed)
        {
            T item = (T)GetItem(session, type, record);
            return Create(session, item, sd, ed);
        }

        public static ActiveLogItem<T> Create(ISession session, T item, DateTime sd, DateTime ed)
        {
            if (item == null) return null;
            IList<ActiveLog> alogs = session.Query<ActiveLog>().Where(x => x.TableName == item.TableName() && x.Record == item.Record() && x.EnableDate < ed && (x.DisableDate == null || x.DisableDate.Value > sd)).ToList();
            return Create(item, alogs);
        }

        public static IEnumerable<ActiveLogItem<T>> Create(ISession session, IQueryable<T> query, IEnumerable<int> records, Expression<Func<T, ActiveLogKey>> key, DateTime sd, DateTime ed)
        {
            var join = JoinActiveLog(session, query, key);
            var result = join.Where(x => records.Contains(x.ActiveLog.Record) && x.ActiveLog.EnableDate < ed && (x.ActiveLog.DisableDate == null || x.ActiveLog.DisableDate.Value > sd)).ToList();
            return result;
        }

        public static IEnumerable<ActiveLogItem<T>> Create(ISession session, IQueryable<T> query, Expression<Func<T, ActiveLogKey>> key, DateTime sd, DateTime ed)
        {
            var join = JoinActiveLog(session, query, key);
            var result = join.Where(x => x.ActiveLog.EnableDate < ed && (x.ActiveLog.DisableDate == null || x.ActiveLog.DisableDate.Value > sd)).ToList();
            return result;
        }

        private static IQueryable<ActiveLogItem<T>> JoinActiveLog(ISession session, IQueryable<T> query, Expression<Func<T, ActiveLogKey>> key)
        {
            return query.Join(session.Query<ActiveLog>()
                    , key
                    , i => new ActiveLogKey(i.TableName, i.Record)
                    , (o, i) => new { Outer = o, Inner = i })
                .Select(x => new ActiveLogItem<T>() { Item = x.Outer, ActiveLog = x.Inner });
        }

        public static ActiveLogItem<T> Create(T item, IList<ActiveLog> alogs)
        {
            if (item == null) return null;
            ActiveLog alog = alogs.FirstOrDefault(x => x.Record == item.Record());
            if (alog == null) return null;
            ActiveLogItem<T> result = new ActiveLogItem<T>(item, alog);
            return result;
        }

        public static ActiveLogType GetActiveLogType(string tableName)
        {
            ActiveLogType result = (ActiveLogType)Enum.Parse(typeof(ActiveLogType), tableName);
            return result;
        }

        public T GetItem(ISession session, string tableName, int record)
        {
            return (T)GetItem(session, GetActiveLogType(tableName), record);
        }

        public static object GetItem(ISession session, ActiveLogType type, int record)
        {
            switch (type)
            {
                case ActiveLogType.Client:
                    return session.Single<Client>(record);
                case ActiveLogType.ClientAccount:
                    return session.Single<ClientAccount>(record);
                case ActiveLogType.ClientManager:
                    return session.Single<ClientManager>(record);
                case ActiveLogType.ClientOrg:
                    return session.Single<ClientOrg>(record);
                case ActiveLogType.ClientRemote:
                    return session.Single<ClientRemote>(record);
                case ActiveLogType.Org:
                    return session.Single<Org>(record);
                default:
                    throw new ArgumentException("type");
            }
        }
    }

    public struct ActiveLogKey
    {
        public ActiveLogKey(string tableName, int record)
        {
            TableName = tableName;
            Record = record;
        }

        public string TableName { get; }
        public int Record { get; }
    }
}
