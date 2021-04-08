using LNF.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;

namespace LNF.DataAccess
{
    public abstract class SimpleRepository : IDisposable
    {
        private static ObjectCache _cache;

        static SimpleRepository()
        {
            _cache = new MemoryCache("SimpleRepositoryCache");
        }

        private SqlConnection _conn;

        public SimpleRepository()
        {
            _conn = NewConnection();
        }

        public IClient GetUser<T>(string username) where T : IClient, new()
        {
            var clients = GetCache<Dictionary<string, IClient>>("clients");

            if (clients == null)
            {
                clients = new Dictionary<string, IClient>();
                SetCache("clients", clients, TimeSpan.FromHours(1));
            }

            if (clients.ContainsKey(username))
                return clients[username];

            using (var cmd = NewCommand())
            {
                cmd.CommandText = "SELECT * FROM sselData.dbo.v_ClientInfo WHERE UserName = @UserName";
                cmd.Parameters.AddWithValue("UserName", username);
                var dt = ExecuteQuery(cmd);
                if (dt.Rows.Count > 0)
                {
                    IClient result = CreateObject<T>(dt.Rows[0]);
                    clients.Add(username, result);
                    return result;
                }
            }

            return null;
        }

        public IAccount GetAccount<T>(int accountId) where T : IAccount, new()
        {
            DataRow[] rows = GetAllAccounts().Select($"AccountID = {accountId}");
            if (rows.Length > 0)
                return CreateObject<T>(rows[0]);
            else
                return null;
        }

        public List<IClient> GetActiveClients<T>() where T : IClient, new()
        {
            DataTable dt = GetAllClients();
            DataRow[] rows = dt.Select("ClientActive = 1");
            List<T> list = CreateObjects<T>(rows).ToList();
            List<IClient> result = new List<IClient>();
            foreach (var item in list)
                result.Add(item);
            return result;
        }

        public List<IRoom> GetActiveRooms<T>() where T : IRoom, new()
        {
            DataTable dt = GetAllRooms();
            DataRow[] rows = dt.Select("Active = 1");
            List<T> list = CreateObjects<T>(rows).ToList();
            List<IRoom> result = new List<IRoom>();
            foreach (var item in list)
                result.Add(item);
            return result;
        }

        public IRoom GetRoom<T>(int roomId) where T : IRoom, new()
        {
            DataTable dt = GetAllRooms();
            DataRow[] rows = dt.Select($"RoomID = {roomId}");
            if (rows != null && rows.Length > 0)
                return CreateObject<T>(rows[0]);
            else
                return null;
        }

        public List<IRoom> GetChildRooms<T>(int parentId) where T : IRoom, new()
        {
            var active = GetActiveRooms<T>().Where(x => x.ParentID == parentId).ToList();
            return active;
        }

        private DataTable GetAllRooms()
        {
            var allRooms = GetCache<DataTable>("all_rooms");

            if (allRooms == null)
            {
                using (var cmd = NewCommand())
                {
                    cmd.CommandText = "SELECT RoomID, ParentID, Room AS RoomName, DisplayName AS RoomDisplayName, PassbackRoom, Billable, ApportionDailyFee, ApportionEntryFee, Active FROM sselData.dbo.Room";
                    allRooms = ExecuteQuery(cmd);
                    SetCache("all_rooms", allRooms);
                }
            }

            return allRooms;
        }

        private DataTable GetAllClients()
        {
            var allClients = GetCache<DataTable>("all_clients");

            if (allClients == null)
            {
                using (var cmd = NewCommand())
                {
                    cmd.CommandText = "SELECT * FROM sselData.dbo.v_ClientInfo";
                    allClients = ExecuteQuery(cmd);
                    SetCache("all_clients", allClients, TimeSpan.FromHours(1));
                }
            }

            return allClients;
        }

        private DataTable GetAllAccounts()
        {
            var allAccounts = GetCache<DataTable>("all_accounts");

            if (allAccounts == null)
            {
                using (var cmd = NewCommand())
                {
                    cmd.CommandText = "SELECT * FROM sselData.dbo.v_AccountInfo";
                    allAccounts = ExecuteQuery(cmd);
                    SetCache("all_accounts", allAccounts, TimeSpan.FromHours(1));
                }
            }

            return allAccounts;
        }

        private SqlConnection NewConnection()
        {
            var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString);
            return conn;
        }

        protected SqlCommand NewCommand()
        {
            if (_conn.State != ConnectionState.Open)
                _conn.Open();

            return _conn.CreateCommand();
        }

        protected DataTable ExecuteQuery(SqlCommand cmd)
        {
            using (var adap = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                adap.Fill(dt);
                return dt;
            }
        }

        public void Dispose()
        {
            if (_conn != null)
            {
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
                _conn.Dispose();
            }
        }

        protected List<T> CreateObjects<T>(DataRow[] rows) where T : new()
        {
            var result = new List<T>();

            foreach (DataRow dr in rows)
            {
                result.Add(CreateObject<T>(dr));
            }

            return result;
        }

        protected List<T> CreateObjects<T>(DataTable dt) where T : new()
        {
            var result = new List<T>();

            foreach (DataRow dr in dt.Rows)
            {
                result.Add(CreateObject<T>(dr));
            }

            return result;
        }

        protected T CreateObject<T>(DataRow dr) where T : new()
        {
            var result = new T();

            var props = typeof(T).GetProperties();

            foreach (var p in props)
            {
                object val = null;

                if (dr.Table.Columns.Contains(p.Name))
                    val = dr[p.Name];

                if (p.CanWrite && val != null && val != DBNull.Value)
                    p.SetValue(result, val);
            }

            return result;
        }

        protected void SetCache(string key, object value, TimeSpan expires = default(TimeSpan))
        {
            if (expires == default(TimeSpan))
                expires = TimeSpan.FromHours(24);

            DateTimeOffset absoluteExpiration = DateTimeOffset.Now.Add(expires);

            CacheItemPolicy policy = new CacheItemPolicy { AbsoluteExpiration = absoluteExpiration };

            _cache.Set(key, value, policy);
        }

        protected T GetCache<T>(string key) where T : class
        {
            object value = _cache[key];

            if (value == null)
                return null;

            return value as T;
        }

        protected object RemoveCache(string key)
        {
            return _cache.Remove(key);
        }
    }
}
