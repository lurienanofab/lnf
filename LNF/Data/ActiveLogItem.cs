using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
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
        private T _Item;
        private ActiveLog _ActiveLog;

        public ISession Session { get; }
        public T Item { get { return _Item; } }
        public ActiveLog ActiveLog { get { return _ActiveLog; } }

        private ActiveLogItem(T item, ActiveLog alog, ISession session)
        {
            _Item = item;
            _ActiveLog = alog;
            Session = session;
        }

        public static ActiveLogItem<T> Create(string tableName, int record, DateTime sdate, DateTime edate, ISession session)
        {
            return Create(GetActiveLogType(tableName), record, sdate, edate, session);
        }

        public static ActiveLogItem<T> Create(ActiveLogType type, int record, DateTime sdate, DateTime edate, ISession session)
        {
            T item = (T)GetItem(type, record, session);
            return Create(item, sdate, edate, session);
        }

        public static ActiveLogItem<T> Create(T item, DateTime sdate, DateTime edate, ISession session)
        {
            if (item == null) return null;
            IList<ActiveLog> alogs = session.Query<ActiveLog>().Where(x => x.TableName == item.TableName() && x.Record == item.Record() && x.EnableDate < edate && (x.DisableDate == null || x.DisableDate.Value > sdate)).ToList();
            return Create(item, alogs, session);
        }

        public static ActiveLogItem<T> Create(T item, IList<ActiveLog> alogs, ISession session)
        {
            if (item == null) return null;
            ActiveLog alog = alogs.FirstOrDefault(x => x.Record == item.Record());
            if (alog == null) return null;
            ActiveLogItem<T> result = new ActiveLogItem<T>(item, alog, session);
            return result;
        }

        public static ActiveLogType GetActiveLogType(string tableName)
        {
            ActiveLogType result = (ActiveLogType)Enum.Parse(typeof(ActiveLogType), tableName);
            return result;
        }

        public T GetItem(string tableName, int record, ISession session)
        {
            return (T)GetItem(GetActiveLogType(tableName), record, session);
        }

        public static object GetItem(ActiveLogType type, int record, ISession session)
        {
            switch (type)
            {
                case ActiveLogType.Client:
                    return session.Single<Client>(record);
                case ActiveLogType.ClientAccount:
                    return session.Single<ClientAccount>(record);
                case ActiveLogType.ClientManager:
                    return session.Single<Repository.Data.ClientManager>(record);
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
}
