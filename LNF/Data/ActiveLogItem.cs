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

        public T Item { get { return _Item; } }
        public ActiveLog ActiveLog { get { return _ActiveLog; } }

        private ActiveLogItem(T item, ActiveLog alog)
        {
            _Item = item;
            _ActiveLog = alog;
        }

        public static ActiveLogItem<T> Create(string tableName, int record, DateTime sdate, DateTime edate)
        {
            return Create(ActiveLogItem<T>.GetActiveLogType(tableName), record, sdate, edate);
        }

        public static ActiveLogItem<T> Create(ActiveLogType type, int record, DateTime sdate, DateTime edate)
        {
            T item = (T)GetItem(type, record);
            return Create(item, sdate, edate);
        }

        public static ActiveLogItem<T> Create(T item, DateTime sdate, DateTime edate)
        {
            if (item == null) return null;
            IList<ActiveLog> alogs = DA.Current.Query<ActiveLog>().Where(x => x.TableName == item.TableName() &&  x.Record == item.Record() && x.EnableDate < edate && (x.DisableDate == null || x.DisableDate.Value > sdate)).ToList();
            return Create(item, alogs);
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

        public static T GetItem(string tableName, int record)
        {
            return (T)GetItem(GetActiveLogType(tableName), record);
        }

        public static object GetItem(ActiveLogType type, int record)
        {
            switch (type)
            {
                case ActiveLogType.Client:
                    return DA.Current.Single<Client>(record);
                case ActiveLogType.ClientAccount:
                    return DA.Current.Single<ClientAccount>(record);
                case ActiveLogType.ClientManager:
                    return DA.Current.Single<ClientManager>(record);
                case ActiveLogType.ClientOrg:
                    return DA.Current.Single<ClientOrg>(record);
                case ActiveLogType.ClientRemote:
                    return DA.Current.Single<ClientRemote>(record);
                case ActiveLogType.Org:
                    return DA.Current.Single<Org>(record);
                default:
                    throw new ArgumentException("type");
            }
        }
    }
}
