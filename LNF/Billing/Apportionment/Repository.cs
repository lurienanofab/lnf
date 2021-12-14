using LNF.Billing.Apportionment.Models;
using LNF.DataAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LNF.Billing.Apportionment
{
    public class Repository : SimpleRepository
    {
        public Repository(SqlConnection conn) : base(conn) { }

        private DataTable _allApportionmentDefaults = null;
        private readonly Dictionary<string, DataTable> _toolDataByPeriod = new Dictionary<string, DataTable>();
        private readonly Dictionary<string, DataTable> _roomDataByPeriod = new Dictionary<string, DataTable>();
        private readonly Dictionary<string, DataTable> _roomBillingByPeriod = new Dictionary<string, DataTable>();
        private readonly Dictionary<string, DataTable> _userApportionmentByPeriod = new Dictionary<string, DataTable>();

        private DataTable GetAllApportionmentClients()
        {
            var dt = GetCache<DataTable>("AllApportionmentClients");

            if (dt == null)
            {
                using (var cmd = NewCommand("SELECT ClientID, UserName, Privs, LName, MName, FName, Active FROM sselData.dbo.Client"))
                {
                    dt = ExecuteQuery(cmd);
                    SetCache("AllApportionmentClients", dt, TimeSpan.FromMinutes(15));
                }
            }

            return dt;
        }

        private DataTable GetAllApportionmentAccounts()
        {
            var dt = GetCache<DataTable>("AllApportionmentAccounts");

            if (dt == null)
            {
                using (var cmd = NewCommand("SELECT a.AccountID, a.[Name] AS AccountName, a.ShortCode, o.OrgID, o.OrgName, a.Active FROM sselData.dbo.Account a INNER JOIN sselData.dbo.Org o ON o.OrgID = a.OrgID"))
                {
                    dt = ExecuteQuery(cmd);
                    SetCache("AllApportionmentAccounts", dt, TimeSpan.FromMinutes(15));
                }
            }

            return dt;
        }


        private DataTable GetAllApportionmentRooms()
        {
            var dt = GetCache<DataTable>("AllApportionmentRooms");

            if (dt == null)
            {
                using (var cmd = NewCommand("SELECT RoomID, ParentID, Room AS RoomName, DisplayName AS RoomDisplayName, Billable, ApportionDailyFee, ApportionEntryFee, Active FROM sselData.dbo.Room"))
                {
                    dt = ExecuteQuery(cmd);
                    SetCache("AllApportionmentRooms", dt, TimeSpan.FromMinutes(15));
                }
            }

            return dt;
        }

        public ApportionmentClient GetApportionmentClient(string username)
        {
            var dt = GetAllApportionmentClients();

            var rows = dt.Select($"UserName = '{username}'");

            if (rows.Length > 0)
                return CreateObject<ApportionmentClient>(rows[0]);
            else
                return null;
        }

        /// <summary>
        /// This will return a dataset with three tables
        /// #1 table: This user's apportion data based on room
        /// #2 table: The actual minimum physical days (by UNION the physical days in lab and account days of using tools)
        /// #3 table: User's Apportion Data
        /// </summary>
        public DataSet GetDataForApportion(DateTime period, int clientId, int roomId)
        {
            using (var cmd = NewCommand("sselData.dbo.RoomApportionmentInDaysMonthly_Select"))
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "ForApportion");
                cmd.Parameters.AddWithValue("Period", period);
                cmd.Parameters.AddWithValue("ClientID", clientId);
                cmd.Parameters.AddWithValue("RoomID", roomId);
                DataSet ds = new DataSet();
                adap.Fill(ds);
                return ds;
            }
        }

        public ApportionmentAccount GetApportionmentAccount(int accountId)
        {
            var dt = GetAllApportionmentAccounts();

            var rows = dt.Select($"AccountID = {accountId}");

            if (rows.Length > 0)
                return CreateObject<ApportionmentAccount>(rows[0]);
            else
                return null;
        }

        public List<ApportionmentClient> GetActiveApportionmentClients()
        {
            var dt = GetAllApportionmentClients();

            var rows = dt.Select("Active = 1");

            if (rows.Length > 0)
                return CreateObjects<ApportionmentClient>(rows);
            else
                return null;
        }

        public List<ApportionmentRoom> GetActiveApportionmentRooms()
        {
            var dt = GetAllApportionmentRooms();

            var rows = dt.Select("Active = 1");

            if (rows.Length > 0)
                return CreateObjects<ApportionmentRoom>(rows);
            else
                return null;
        }

        public ApportionmentRoom GetApportionmentRoom(int roomId)
        {
            var dt = GetAllApportionmentRooms();

            var rows = dt.Select($"RoomID = {roomId}");

            if (rows.Length > 0)
                return CreateObject<ApportionmentRoom>(rows[0]);
            else
                return null;
        }

        public List<ApportionmentRoom> GetChildApportionmentRooms(int parentId)
        {
            var dt = GetAllApportionmentRooms();

            var rows = dt.Select($"ParentID = {parentId} AND Active = 1");

            if (rows.Length > 0)
                return CreateObjects<ApportionmentRoom>(rows);
            else
                return null;
        }

        private DataTable GetAllApportionmentDefaults()
        {
            if (_allApportionmentDefaults == null)
            {
                using (var cmd = NewCommand())
                {
                    cmd.CommandText = "SELECT * FROM sselData.dbo.ApportionmentDefault";
                    _allApportionmentDefaults = ExecuteQuery(cmd);
                }
            }

            return _allApportionmentDefaults;
        }

        public DataTable GetToolDataByPeriod(DateTime period)
        {
            DataTable dt;

            string key = period.ToString("yyyy-MM-dd");

            if (!_toolDataByPeriod.ContainsKey(key))
            {
                using (var cmd = NewCommand())
                {
                    cmd.CommandText = "SELECT td.*, acct.OrgID FROM sselData.dbo.ToolData td INNER JOIN sselData.dbo.Account acct ON acct.AccountID = td.AccountID WHERE td.[Period] = @Period";
                    cmd.Parameters.AddWithValue("Period", period);
                    dt = ExecuteQuery(cmd);
                    _toolDataByPeriod.Add(key, dt);
                }
            }
            else
            {
                dt = _toolDataByPeriod[key];
            }

            return dt;
        }

        public void SaveDefaultApportionment(int clientId, IEnumerable<DefaultApportionment> items)
        {
            using (var cmd = NewCommand())
            {
                cmd.CommandText = "DELETE sselData.dbo.ApportionmentDefault WHERE ClientID = @ClientID";
                cmd.Parameters.AddWithValue("ClientID", clientId, SqlDbType.Int);
                cmd.ExecuteNonQuery();
            }

            using (var cmd = NewCommand())
            {
                cmd.CommandText = "INSERT sselData.dbo.ApportionmentDefault (ClientID, RoomID, AccountID, Percentage) VALUES (@ClientID, @RoomID, @AccountID, @Percentage)";

                cmd.Parameters.AddWithValue("ClientID", clientId, SqlDbType.Int);
                cmd.Parameters.Add("RoomID", SqlDbType.Int);
                cmd.Parameters.Add("AccountID", SqlDbType.Int);
                cmd.Parameters.Add("Percentage", SqlDbType.Float);

                foreach (var i in items)
                {
                    cmd.Parameters["RoomID"].Value = i.RoomID;
                    cmd.Parameters["AccountID"].Value = i.AccountID;
                    cmd.Parameters["Percentage"].Value = i.Percentage;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public DataTable GetRoomDataByPeriod(DateTime period)
        {
            DataTable dt;

            string key = period.ToString("yyyy-MM-dd");

            if (!_roomDataByPeriod.ContainsKey(key))
            {
                using (var cmd = NewCommand())
                {
                    cmd.CommandText = "SELECT * FROM sselData.dbo.RoomData WHERE [Period] = @Period";
                    cmd.Parameters.AddWithValue("Period", period);
                    dt = ExecuteQuery(cmd);
                    _roomDataByPeriod.Add(key, dt);
                }
            }
            else
            {
                dt = _roomDataByPeriod[key];
            }

            return dt;
        }

        public DataTable GetRoomBillingByPeriod(DateTime period)
        {
            DataTable dt;

            string key = period.ToString("yyyy-MM-dd");

            if (!_roomBillingByPeriod.ContainsKey(key))
            {
                using (var cmd = NewCommand())
                {
                    cmd.CommandText = "SELECT rb.*, acct.Name AS AccountName, acct.Number AS AccountNumber, org.OrgName, room.ParentID FROM sselData.dbo.RoomApportionmentInDaysMonthly rb INNER JOIN sselData.dbo.Account acct ON acct.AccountID = rb.AccountID INNER JOIN sselData.dbo.Org org ON org.OrgID = rb.OrgID INNER JOIN sselData.dbo.Room room ON room.RoomID = rb.RoomID WHERE rb.[Period] = @Period";
                    cmd.Parameters.AddWithValue("Period", period);
                    dt = ExecuteQuery(cmd);
                    _roomBillingByPeriod.Add(key, dt);
                }
            }
            else
            {
                dt = _roomBillingByPeriod[key];
            }

            return dt;
        }

        public DataTable GetUserApportionmentByPeriod(DateTime period)
        {
            DataTable dt;

            string key = period.ToString("yyyy-MM-dd");

            if (!_userApportionmentByPeriod.ContainsKey(key))
            {
                using (var cmd = NewCommand())
                {
                    cmd.CommandText = "SELECT uapp.*, room.ParentID FROM sselData.dbo.RoomBillingUserApportionData uapp INNER JOIN sselData.dbo.Room room ON room.RoomID = uapp.RoomID WHERE uapp.[Period] = @Period";
                    cmd.Parameters.AddWithValue("Period", period);
                    dt = ExecuteQuery(cmd);
                    _userApportionmentByPeriod.Add(key, dt);
                }
            }
            else
            {
                dt = _userApportionmentByPeriod[key];
            }

            return dt;
        }

        public double GetAccountEntries(DateTime period, int clientId, int roomId, int accountId)
        {
            DataTable dt = GetRoomBillingByPeriod(period);

            object obj = dt.Compute("SUM(Entries)", $"ClientID = {clientId} AND RoomID = {roomId} AND AccountID = {accountId}");

            double result = 0;

            if (obj != null && obj != DBNull.Value)
                result = Convert.ToDouble(obj);

            return result;
        }

        public double GetDefaultApportionmentPercentage(int clientId, int roomId, int accountId)
        {
            DataTable dt = GetAllApportionmentDefaults();

            DataRow[] rows = dt.Select($"ClientID = {clientId} AND RoomID = {roomId} AND AccountID = {accountId}");

            double result = 0;

            if (rows != null && rows.Length > 0)
                result = Convert.ToDouble(rows[0]["Percentage"]);

            return result;
        }

        public ArrayList GetAccountDaysAndPhysicalDays(DateTime period, int clientId, int roomId, int accountId)
        {
            using (var cmd = NewCommand())
            {
                cmd.CommandText = "sselData.dbo.RoomApportionmentInDaysMonthly_Select";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "ForApportion");
                cmd.Parameters.AddWithValue("Period", period);
                cmd.Parameters.AddWithValue("ClientID", clientId);
                cmd.Parameters.AddWithValue("RoomID", roomId);
                cmd.Parameters.AddWithValue("AccountID", accountId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var arr = new ArrayList
                        {
                            reader["PhysicalDays"],
                            reader["AccountDays"]
                        };
                        return arr;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public int[] GetRoomBillingAccountIds(DateTime period, int clientId, int roomId)
        {
            DataTable dt = GetRoomBillingByPeriod(period);
            DataRow[] rows = dt.Select($"ClientID = {clientId} AND RoomID = {roomId}");

            int[] result;

            if (rows == null || rows.Length == 0)
                result = new int[0];
            else
                result = rows.Select(x => x.Field<int>("AccountID")).ToArray();

            return result;
        }

        public int UpdateRoomApportionmentInDaysMonthly(DataTable dt)
        {
            //Update the data using dateset's batch update feature

            int result = 0;

            using (var update = NewCommand())
            {
                update.CommandType = CommandType.StoredProcedure;
                update.CommandText = "sselData.dbo.RoomApportionmentInDaysMonthly_Update";
                update.Parameters.Add("AppID", SqlDbType.Int);
                update.Parameters.Add("ChargeDays", SqlDbType.Float);
                update.Parameters.Add("Entries", SqlDbType.Float);
                update.Parameters.Add("Hours", SqlDbType.Float);
                update.Parameters.Add("MonthlyRoomCharge", SqlDbType.Float);
                update.Parameters.Add("IsDefault", SqlDbType.Bit);

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.RowState == DataRowState.Modified)
                    {
                        update.Parameters["AppID"].Value = dr["AppID"];
                        update.Parameters["ChargeDays"].Value = dr["ChargeDays"];
                        update.Parameters["Entries"].Value = dr["Entries"];
                        update.Parameters["Hours"].Value = dr["Hours"];
                        update.Parameters["MonthlyRoomCharge"].Value = dr["MonthlyRoomCharge"];
                        update.Parameters["IsDefault"].Value = dr["IsDefault"];
                        result += update.ExecuteNonQuery();
                        dr.AcceptChanges();
                    }
                }

                return result;
            }
        }

        public int SaveRoomBillingUserApportionData(DataTable dt)
        {
            using (var insert = NewCommand())
            using (var update = NewCommand())
            {
                insert.CommandType = CommandType.StoredProcedure;
                insert.CommandText = "sselData.dbo.RoomBillingUserApportionData_Insert";
                insert.Parameters.Add("Period", SqlDbType.DateTime);
                insert.Parameters.Add("ClientID", SqlDbType.Int);
                insert.Parameters.Add("RoomID", SqlDbType.Int);
                insert.Parameters.Add("AccountID", SqlDbType.Int);
                insert.Parameters.Add("ChargeDays", SqlDbType.Float);
                insert.Parameters.Add("Entries", SqlDbType.Float);

                update.CommandType = CommandType.StoredProcedure;
                update.CommandText = "sselData.dbo.RoomBillingUserApportionData_Update";
                update.Parameters.Add("AppID", SqlDbType.Int);
                update.Parameters.Add("ChargeDays", SqlDbType.Float);

                int result = 0;

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.RowState == DataRowState.Added)
                    {
                        insert.Parameters["Period"].Value = dr["Period"];
                        insert.Parameters["ClientID"].Value = dr["ClientID"];
                        insert.Parameters["RoomID"].Value = dr["RoomID"];
                        insert.Parameters["AccountID"].Value = dr["AccountID"];
                        insert.Parameters["ChargeDays"].Value = dr["ChargeDays"];
                        insert.Parameters["Entries"].Value = dr["Entries"];
                        result += insert.ExecuteNonQuery();
                        dr.AcceptChanges();
                    }

                    if (dr.RowState == DataRowState.Modified)
                    {
                        update.Parameters["AppID"].Value = dr["AppID"];
                        update.Parameters["ChargeDays"].Value = dr["ChargeDays"];
                        result += update.ExecuteNonQuery();
                        dr.AcceptChanges();
                    }
                }

                return result;
            }
        }

        public void UpdateRoomBillingEntries(DateTime period, int clientId, int roomId, int accountId, decimal entries)
        {
            DataRow[] rows = GetRoomBillingByPeriod(period).Select($"ClientID = {clientId} AND RoomID = {roomId} AND AccountID = {accountId}");

            if (rows == null || rows.Length == 0)
                throw new Exception($"Cannot find RoomBilling record for Period = #{period:yyyy-MM-dd}#, ClientID = {clientId}, RoomID = {roomId}, AccountID = {accountId}");

            DataRow rb = rows[0];

            using (var cmd = NewCommand())
            {
                int appId = rb.Field<int>("AppID");
                cmd.CommandText = "UPDATE sselData.dbo.RoomApportionmentInDaysMonthly SET Entries = @Entries WHERE AppID = @AppID";
                cmd.Parameters.AddWithValue("AppID", appId);
                cmd.Parameters.AddWithValue("Entries", entries);
                cmd.ExecuteNonQuery();
            }

            var userData = GetUserApportionmentByPeriod(period).Select($"ClientID = {clientId} AND RoomID = {roomId} AND AccountID = {accountId}");

            if (userData == null || userData.Length == 0)
            {
                int cid = rb.Field<int>("ClientID");
                int rid = rb.Field<int>("RoomID");
                int aid = rb.Field<int>("AccountID");
                InsetRoomBillingUserApportionData(period, cid, rid, aid, 0, 0);
            }
            else
            {
                int appId = userData[0].Field<int>("AppID");
                decimal chargeDays = Convert.ToDecimal(userData[0].Field<double>("ChargeDays"));
                UpdateRoomBillingUserApportionData(appId, entries, chargeDays);
            }
        }

        public int UpdateChildRoomEntryApportionment(DateTime period, int clientId, int parentRoomId)
        {
            int result = 0;

            var parentRoom = GetApportionmentRoom(parentRoomId);

            //make sure this really is a parent room
            if (parentRoom.ParentID == null)
            {
                //need to know the child rooms
                var children = GetChildApportionmentRooms(parentRoomId).ToArray();

                //continue only if there are children rooms
                if (children.Length > 0)
                {
                    //entries and physical days for each child room
                    var entries = GetRoomDataByPeriod(period).Select($"ClientID = {clientId} AND ParentID = {parentRoomId}")
                        .Select(x => new ApportionmentRoomData
                        {
                            RoomID = x.Field<int>("RoomID"),
                            Entries = x.Field<double>("Entries"),
                            EvtDate = x.Field<DateTime>("EvtDate").ToString("yyyy-MM-dd")
                        })
                        .GroupBy(x => x.RoomID)
                        .Select(g => new ApportionmentEntry
                        {
                            RoomID = g.Key,
                            TotalEntries = g.Sum(n => n.Entries),
                            PhysicalDays = Convert.ToDecimal(g.Select(n => n.EvtDate).Distinct().Count())
                        })
                        .ToArray();

                    var dtRoomBilling = GetRoomBillingByPeriod(period);

                    //child room apportionment records, this is what we'll be updating
                    var childAppor = dtRoomBilling.Select($"ClientID = {clientId} AND ParentID = {parentRoomId}");

                    //also update the child user apportionment data
                    var childUserAppor = GetUserApportionmentByPeriod(period).Select($"ClientID = {clientId} AND ParentID = {parentRoomId}");

                    //parent room apportionment records
                    var parentAppor = dtRoomBilling.Select($"ClientID = {clientId} AND RoomID = {parentRoomId}");

                    var totalParentChargeDays = parentAppor.Sum(x => x.Field<decimal>("ChargeDays"));

                    //get the pct for each acct based on parent room day apporiontment
                    foreach (DataRow drParentAppor in parentAppor)
                    {
                        var parentChargeDays = drParentAppor.Field<decimal>("ChargeDays");
                        var pct = GetPercentage(parentChargeDays, totalParentChargeDays);

                        //get the total for each child room
                        foreach (var child in children)
                        {
                            var e = entries.FirstOrDefault(x => x.RoomID == child.RoomID);

                            //e will be null if they didn't go into a room (e.g. wet chem)
                            if (e != null)
                            {
                                //the pct for this acct
                                decimal chargeDays = Math.Round(e.PhysicalDays * pct, 4, MidpointRounding.AwayFromZero);
                                decimal chargeEntries = Math.Round(Convert.ToDecimal(e.TotalEntries) * pct, 4, MidpointRounding.AwayFromZero);

                                //get the child RoomApportionment record
                                var drChildAppor = childAppor.FirstOrDefault(x => x.Field<int>("RoomID") == child.RoomID && x.Field<int>("AccountID") == drParentAppor.Field<int>("AccountID"));

                                if (drChildAppor != null)
                                {
                                    //update the record
                                    using (var cmd = NewCommand())
                                    {
                                        int appId = drChildAppor.Field<int>("AppID");
                                        cmd.CommandText = "UPDATE sselData.dbo.RoomApportionmentInDaysMonthly SET Entries = @Entries, ChargeDays = @ChargeDays WHERE AppID = @AppID";
                                        cmd.Parameters.AddWithValue("AppID", appId);
                                        cmd.Parameters.AddWithValue("Entries", chargeEntries);
                                        cmd.Parameters.AddWithValue("ChargeDays", chargeDays);
                                        cmd.ExecuteNonQuery();
                                    }

                                    result++;
                                }

                                //update/insert the user apportionment data
                                var userData = childUserAppor.FirstOrDefault(x => x.Field<int>("RoomID") == child.RoomID && x.Field<int>("AccountID") == drParentAppor.Field<int>("AccountID"));

                                if (userData != null)
                                {
                                    //update the existing record
                                    int appId = userData.Field<int>("AppID");
                                    UpdateRoomBillingUserApportionData(appId, chargeEntries, chargeDays);
                                }
                                else
                                {
                                    //create the record if it does not exist
                                    int cid = drParentAppor.Field<int>("ClientID");
                                    int aid = drParentAppor.Field<int>("AccountID");
                                    InsetRoomBillingUserApportionData(period, cid, child.RoomID, aid, chargeDays, chargeEntries);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        private decimal GetPercentage(decimal part, decimal total)
        {
            var temp1 = part / total;
            var temp2 = Math.Round(temp1, 8, MidpointRounding.AwayFromZero);
            var result = Convert.ToDecimal(temp2);
            return result;
        }

        private void UpdateRoomBillingUserApportionData(int appId, decimal chargeEntries, decimal chargeDays)
        {
            using (var cmd = NewCommand())
            {
                cmd.CommandText = "UPDATE sselData.dbo.RoomBillingUserApportionData SET Entries = @Entries, ChargeDays = @ChargeDays WHERE AppID = @AppID";
                cmd.Parameters.AddWithValue("AppID", appId);
                cmd.Parameters.AddWithValue("Entries", chargeEntries);
                cmd.Parameters.AddWithValue("ChargeDays", chargeDays);
                cmd.ExecuteNonQuery();
            }
        }

        private void InsetRoomBillingUserApportionData(DateTime period, int clientId, int roomId, int accountId, decimal chargeDays, decimal entries)
        {
            using (var cmd = NewCommand())
            {
                cmd.CommandText = "INSERT sselData.dbo.RoomBillingUserApportionData ([Period], ClientID, RoomID, AccountID, ChargeDays, Entries) VALUES (@Period, @ClientID, @RoomID, @AccountID, @ChargeDays, @Entries)";
                cmd.Parameters.AddWithValue("Period", period);
                cmd.Parameters.AddWithValue("ClientID", clientId);
                cmd.Parameters.AddWithValue("RoomID", roomId);
                cmd.Parameters.AddWithValue("AccountID", accountId);
                cmd.Parameters.AddWithValue("ChargeDays", chargeDays);
                cmd.Parameters.AddWithValue("Entries", entries);
                cmd.ExecuteNonQuery();
            }
        }

        public RoomEntryApportionmentAccount GetRoomEntryApportionmentAccount(RoomEntryApportionment item, int accountId)
        {
            var acct = GetApportionmentAccount(accountId);

            if (acct == null)
            {
                throw new ItemNotFoundException("Account", "AccountID", accountId);
            }

            double entries = GetAccountEntries(item.Period, item.ClientID, item.RoomID, acct.AccountID);
            double defaultPercentage = GetDefaultApportionmentPercentage(item.ClientID, item.RoomID, acct.AccountID);

            var result = new RoomEntryApportionmentAccount
            {
                Period = item.Period,
                ClientID = item.ClientID,
                RoomID = item.RoomID,
                AccountID = acct.AccountID,
                AccountName = acct.AccountName,
                ShortCode = acct.ShortCode,
                OrgID = acct.OrgID,
                OrgName = acct.OrgName,
                Entries = entries,
                DefaultPercentage = defaultPercentage
            };

            return result;
        }

        public RoomEntryApportionment GetRoomEntryApportionmentModel(DateTime period, int clientId, ApportionmentRoom r)
        {
            string displayName = string.IsNullOrEmpty(r.RoomDisplayName) ? r.RoomName : r.RoomDisplayName;
            double totalEntries = GetTotalEntries(period, clientId, r.RoomID);

            var result = new RoomEntryApportionment
            {
                Period = period,
                ClientID = clientId,
                RoomID = r.RoomID,
                RoomName = r.RoomName,
                DisplayName = displayName,
                TotalEntries = totalEntries
            };

            return result;
        }

        public double GetTotalEntries(DateTime period, int clientId, int roomId)
        {
            var obj = GetRoomDataByPeriod(period).Compute("SUM(Entries)", $"ClientID = {clientId} AND (RoomID = {roomId} OR ParentID = {roomId})");
            if (obj != null && obj != DBNull.Value)
                return Convert.ToDouble(obj);
            else
                return 0;
        }

        public int GetPhysicalDays(DateTime period, int clientId, int roomId)
        {
            var roomData = GetRoomDataByPeriod(period).Select($"ClientID = {clientId} AND (RoomID = {roomId} OR ParentID = {roomId})");
            return roomData.Select(x => x.Field<DateTime>("EvtDate")).Distinct().Count();
        }

        public int GetMinimumDays(DateTime period, int clientId, int roomId, int orgId)
        {
            var toolData = GetToolDataByPeriod(period).Select($"ClientID = {clientId} AND IsActive = 1 AND IsStarted = 1 AND ChargeMultiplier > 0");

            var actDates = toolData
                .Where(x => IsInRoom(x, roomId) && IsInOrg(x, orgId))
                .Select(x => new { ReservationID = x.Field<int>("ReservationID"), ActDate = x.Field<DateTime>("ActDate") })
                .OrderBy(x => x.ActDate)
                .ToList();

            var distinct = actDates.Select(x => x.ActDate.ToString("yyyy-MM-dd")).Distinct().ToArray();

            var result = distinct.Length;

            return result;
        }

        public bool IsInRoom(DataRow dr, int roomId)
        {
            // It is possible that td.RoomID is null, however this hasn't happened since Dec 2007 so it
            // probably won't happen again, but the column does allow nulls.
            if (dr["RoomID"] == DBNull.Value) return false;

            int rid = dr.Field<int>("RoomID");

            // get the room for this td
            var r = GetApportionmentRoom(rid);

            // Sometimes there is a room (e.g. Conference Room) with a non-null RoomID that does not 
            // have an entry in the Room table. In this case td.RoomID != roomId.
            if (r == null)
                return false;

            // check if roomId is either r.RoomID or r.ParentID
            return roomId == r.RoomID || roomId == r.ParentID;
        }

        public bool IsInOrg(DataRow dr, int orgId)
        {
            // check if the orgId is a.OrgID
            return orgId == dr.Field<int>("OrgID");
        }
    }
}
