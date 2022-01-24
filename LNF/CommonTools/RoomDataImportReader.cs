using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LNF.CommonTools
{
    /// <summary>
    /// Replaces sselData.dbo.NexWatch_Select @Action='RoomDataRaw'
    /// </summary>
    public class RoomDataImportReader : IDisposable
    {
        // Replacing stored proc because it takes a ridiculously long time to run, probably because of cursors.

        // This relies on Billing.dbo.RoomDataImport having rows in it for the requested date range. Run ImportRoomData()
        // to get the latest records from Prowatch. Normally this is run every 30 minutes by an agent job.

        private readonly SqlConnection _conn;
        private IList<RoomDataImportItem> _items;

        private static readonly Dictionary<int, string> _altrooms = new Dictionary<int, string>() { [6] = "Entrance to service corridor" };

        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public int ClientID { get; set; }
        public int RoomID { get; set; }
        public int Count => Items.Count();
        public IEnumerable<RoomDataImportCost> Costs { get; }
        public IEnumerable<RoomDataImportRoom> Rooms { get; }
        public IEnumerable<RoomDataImportItem> Items => AllItems().Where(x => !x.Deleted).OrderBy(x => x.ClientID).ThenBy(x => x.RoomID).ThenBy(x => x.EventDate).ToList();
        public int AntipassbackErrorCutoff { get; set; } = 90;

        public IEnumerable<RoomDataImportItem> AllItems() => _items;

        public int[] GetDistinctClients()
        {
            var list = Items.Select(x => x.ClientID).Distinct().ToList();
            list.Sort();
            return list.ToArray();
        }

        public RoomDataImportReader(DateTime sd, DateTime ed, int clientId, int roomId)
        {
            StartDate = sd;
            EndDate = ed;
            ClientID = clientId;
            RoomID = roomId;

            _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString);
            _conn.Open();

            Costs = GetCosts();
            Rooms = GetRooms();
        }

        public string GetRoomName()
        {
            var room = Rooms.First(x => x.RoomID == RoomID);
            return room.RoomName;
        }

        // this is to handle the new service aisle entrance, it should count as a Clean Room entry
        public string GetAltRoomName()
        {
            if (_altrooms.ContainsKey(RoomID))
                return _altrooms[RoomID];
            else
                return "n/a";
        }

        public void ImportRoomData()
        {
            using (var cmd = _conn.CreateCommand("Billing.dbo.ImportRoomData", useConnectionTimeout: false))
            {
                cmd.CommandTimeout = 300;
                cmd.ExecuteNonQuery();
            }
        }

        public void SelectRoomDataImportItems()
        {
            using (var cmd = _conn.CreateCommand(string.Empty, CommandType.Text))
            {
                cmd.CommandText = "SELECT rdi.* FROM Billing.dbo.RoomDataImport rdi WHERE rdi.ClientID = ISNULL(@ClientID, rdi.ClientID) AND (rdi.RoomName = ISNULL(@RoomName, rdi.RoomName) OR rdi.RoomName = @AltRoomName) AND rdi.EventDate >= @sd AND rdi.EventDate < @ed ORDER BY rdi.EventDate";
                cmd.Parameters.AddWithValue("sd", StartDate);
                cmd.Parameters.AddWithValue("ed", EndDate);

                if (ClientID > 0)
                    cmd.Parameters.AddWithValue("ClientID", ClientID);
                else
                    cmd.Parameters.AddWithValue("ClientID", DBNull.Value);

                if (RoomID > 0)
                {
                    cmd.Parameters.AddWithValue("RoomName", GetRoomName());
                    cmd.Parameters.AddWithValue("AltRoomName", GetAltRoomName());
                }
                else
                {
                    cmd.Parameters.AddWithValue("RoomName", DBNull.Value);
                    cmd.Parameters.AddWithValue("AltRoomName", "n/a");
                }

                using (var adap = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    adap.Fill(dt);

                    _items = dt.AsEnumerable().Select(CreateRoomDataImportItem).ToList();

                    DeleteInvalidAntipassbackErrors();
                    DeleteInvalidPassbackEvents();
                    DeleteInvalidRooms();
                    UpdateEventDescription("Host Grant - IN", "Local Grant - IN");
                    UpdateEventDescription("Host Grant - OUT", "Local Grant - OUT");
                    UpdateAltRoomNames();
                    InsertPreviousInEvents();
                    InsertNextOutEvents();
                }
            }
        }

        public DataSet AsDataSet()
        {
            // Three tables are returned:
            //      0) RoomDataRaw
            //      1) Client
            //      2) Room
            //      3) Cost (this is new)

            /****** RoomDataRaw Table **********/
            var dtRoomDataRaw = new DataTable("RoomDataRaw");
            dtRoomDataRaw.Columns.Add("ClientID", typeof(int));
            dtRoomDataRaw.Columns.Add("RoomID", typeof(int));
            dtRoomDataRaw.Columns.Add("evtDate", typeof(DateTime));
            dtRoomDataRaw.Columns.Add("evtType", typeof(string));

            foreach (var i in Items)
                dtRoomDataRaw.Rows.Add(i.ClientID, i.RoomID, i.EventDate, i.EventDescription);

            /****** Client Table ***************/
            var dtClient = new DataTable("Client");
            dtClient.Columns.Add("ClientID", typeof(int));

            foreach (var c in GetDistinctClients())
                dtClient.Rows.Add(c);

            /****** Room Table *****************/
            var dtRoom = new DataTable("Room");
            dtRoom.Columns.Add("RoomID", typeof(int));
            dtRoom.Columns.Add("PassbackRoom", typeof(bool));
            dtRoom.Columns.Add("IsChargedRoom", typeof(bool));

            foreach (var r in Rooms)
                dtRoom.Rows.Add(r.RoomID, r.PassbackRoom, r.IsChargedRoom);

            /****** Cost Table *****************/
            var dtCost = new DataTable("Cost");
            dtCost.Columns.Add("CostID", typeof(int));
            dtCost.Columns.Add("ChargeTypeID", typeof(int));
            dtCost.Columns.Add("TableNameOrDescript", typeof(string));
            dtCost.Columns.Add("RecordID", typeof(int));
            dtCost.Columns.Add("AcctPer", typeof(string));
            dtCost.Columns.Add("AddVal", typeof(double));
            dtCost.Columns.Add("MulVal", typeof(double));
            dtCost.Columns.Add("EffDate", typeof(DateTime));

            foreach (var c in Costs)
                dtCost.Rows.Add(c.CostID, c.ChargeTypeID, c.TableNameOrDescript, c.RecordID, c.AcctPer, c.AddVal, c.MulVal, c.EffDate);

            /****** Create DataSet *************/
            var ds = new DataSet();
            ds.Tables.Add(dtRoomDataRaw);
            ds.Tables.Add(dtClient);
            ds.Tables.Add(dtRoom);
            ds.Tables.Add(dtCost);

            return ds;
        }

        // this is to handle the new service aisle entrance, it should count as a Clean Room entry
        private RoomDataImportRoom GetRoom(string roomName)
        {
            int roomId = 0;

            foreach (var kvp in _altrooms)
            {
                if (kvp.Value == roomName)
                {
                    roomId = kvp.Key;
                    break;
                }
            }

            RoomDataImportRoom room;

            if (roomId == 0)
                room = Rooms.FirstOrDefault(x => x.RoomName == roomName);
            else
                room = Rooms.FirstOrDefault(x => x.RoomID == roomId);

            return room;
        }

        private RoomDataImportItem CreateRoomDataImportItem(DataRow dr)
        {
            var actualRoomName = dr["RoomName"].ToString();

            // gets Clean Room if dr["RoomName"] is "Entrance to service corridor"
            var room = GetRoom(actualRoomName);

            int roomId = 0;
            string roomName = string.Empty;
            string roomDisplayName = string.Empty;
            bool passbackRoom = false;
            bool billable = false;
            bool isChargedRoom = false;

            if (room != null)
            {
                roomId = room.RoomID;
                roomName = room.RoomName;
                roomDisplayName = room.RoomDisplayName;
                passbackRoom = room.PassbackRoom;
                billable = room.Billable;
                isChargedRoom = room.IsChargedRoom;
            }

            return new RoomDataImportItem
            {
                RoomDataImportID = dr.Field<int>("RoomDataImportID"),
                RID = dr.Field<byte[]>("RID"),
                ClientID = dr.Field<int>("ClientID"),
                RoomName = roomName,
                EventDate = dr.Field<DateTime>("EventDate"),
                EventDescription = dr.Field<string>("EventDescription"),
                RoomID = roomId,
                RoomDisplayName = roomDisplayName,
                PassbackRoom = passbackRoom,
                Billable = billable,
                IsChargedRoom = isChargedRoom
            };
        }

        private void DeleteInvalidAntipassbackErrors()
        {
            foreach (var i in _items.Where(x => x.IsAntipassbackError()))
            {
                bool hasNonAntipassbackRecordWithinCutoff = _items.Any(
                    x => x.ClientID == i.ClientID
                    && x.RoomID == i.RoomID
                    && !x.IsAntipassbackError()
                    && Math.Abs((x.EventDate - i.EventDate).TotalSeconds) <= AntipassbackErrorCutoff);

                if (hasNonAntipassbackRecordWithinCutoff)
                    i.Deleted = true;
            }
        }

        private void DeleteInvalidPassbackEvents()
        {
            string[] badEvents = { "Local Grant", "Host Grant", "Local Grant - APB Error - Used" };
            var badItems = _items.Where(x => x.PassbackRoom && badEvents.Contains(x.EventDescription));
            foreach (var i in badItems)
                i.Deleted = true;
        }

        private void DeleteInvalidRooms()
        {
            foreach (var i in _items)
            {
                if (!Rooms.Any(x => x.RoomID == i.RoomID))
                    i.Deleted = true;
            }
        }

        private void UpdateEventDescription(string current, string replace)
        {
            foreach (var i in _items.Where(x => x.EventDescription == current))
                i.EventDescription = replace;
        }

        private void UpdateAltRoomNames()
        {
            foreach (var kvp in _altrooms)
            {
                foreach (var i in _items.Where(x => x.RoomID == kvp.Key))
                {

                }
            }
        }

        private void InsertPreviousInEvents()
        {
            // need the min event for each client/room that is not 'Local Grant - IN'

            var group = _items.AsEnumerable()
                .Where(x => !x.Deleted && x.PassbackRoom && x.Billable)
                .GroupBy(x => new { x.ClientID, x.RoomID })
                .Select(g => new
                {
                    g.Key.ClientID,
                    g.Key.RoomID,
                    Item = g.First(x =>
                        x.ClientID == g.Key.ClientID
                        && x.RoomID == g.Key.RoomID
                        && x.EventDate == g.Min(m => m.EventDate))
                })
                .ToList();

            var list = group
                .Where(x => x.Item.EventDescription != "Local Grant - IN")
                .OrderBy(x => x.ClientID)
                .ThenBy(x => x.RoomID)
                .ThenBy(x => x.Item.EventDate)
                .ToList();

            foreach (var i in list)
            {
                var e = GetPreviousInEvent(i.Item);
                _items.Add(e);
            }
        }

        private void InsertNextOutEvents()
        {
            // need the max event for each client/room that is not 'Local Grant - OUT'

            var group = _items.AsEnumerable()
                .Where(x => !x.Deleted && x.PassbackRoom && x.Billable)
                .GroupBy(x => new { x.ClientID, x.RoomID })
                .Select(g => new
                {
                    g.Key.ClientID,
                    g.Key.RoomID,
                    Item = g.Last(x =>
                        x.ClientID == g.Key.ClientID
                        && x.RoomID == g.Key.RoomID
                        && x.EventDate == g.Max(m => m.EventDate))
                })
                .ToList();

            var list = group
                .Where(x => x.Item.EventDescription != "Local Grant - OUT")
                .OrderBy(x => x.ClientID)
                .ThenBy(x => x.RoomID)
                .ThenByDescending(x => x.Item.EventDate)
                .ToList();

            foreach (var i in list)
            {
                var e = GetNextOutEvent(i.Item);
                _items.Add(e);
            }
        }

        private RoomDataImportItem GetPreviousInEvent(RoomDataImportItem item)
        {
            string eventDesc = "Local Grant - IN";

            using (var cmd = _conn.CreateCommand("SELECT TOP (1) rdi.EventDate FROM Billing.dbo.RoomDataImport rdi WHERE rdi.ClientID = @ClientID AND rdi.RoomName = @RoomName AND rdi.EventDate < @sd AND EventDescription = @EventDescription ORDER BY rdi.EventDate DESC", CommandType.Text))
            {
                cmd.Parameters.AddWithValue("ClientID", item.ClientID);
                cmd.Parameters.AddWithValue("RoomName", item.RoomName);
                cmd.Parameters.AddWithValue("EventDescription", eventDesc);
                cmd.Parameters.AddWithValue("sd", StartDate);

                var eventDate = ConvertTo(cmd.ExecuteScalar(), StartDate);

                var result = new RoomDataImportItem
                {
                    RoomDataImportID = 0,
                    RID = new byte[0],
                    ClientID = item.ClientID,
                    RoomName = item.RoomName,
                    EventDate = eventDate,
                    EventDescription = eventDesc,
                    RoomID = item.RoomID,
                    RoomDisplayName = item.RoomDisplayName,
                    PassbackRoom = item.PassbackRoom,
                    Billable = item.Billable,
                    IsChargedRoom = item.IsChargedRoom,
                    Deleted = false
                };

                return result;
            }
        }

        private RoomDataImportItem GetNextOutEvent(RoomDataImportItem item)
        {
            string eventDesc = "Local Grant - OUT";

            using (var cmd = _conn.CreateCommand("SELECT TOP (1) rdi.EventDate FROM Billing.dbo.RoomDataImport rdi WHERE rdi.ClientID = @ClientID AND rdi.RoomName = @RoomName AND rdi.EventDate > @ed AND EventDescription = @EventDescription ORDER BY rdi.EventDate ASC", CommandType.Text))
            {
                cmd.Parameters.AddWithValue("ClientID", item.ClientID);
                cmd.Parameters.AddWithValue("RoomName", item.RoomName);
                cmd.Parameters.AddWithValue("EventDescription", eventDesc);
                cmd.Parameters.AddWithValue("ed", EndDate);

                var eventDate = ConvertTo(cmd.ExecuteScalar(), EndDate);

                var result = new RoomDataImportItem
                {
                    RoomDataImportID = 0,
                    RID = new byte[0],
                    ClientID = item.ClientID,
                    RoomName = item.RoomName,
                    EventDate = eventDate,
                    EventDescription = eventDesc,
                    RoomID = item.RoomID,
                    RoomDisplayName = item.RoomDisplayName,
                    PassbackRoom = item.PassbackRoom,
                    Billable = item.Billable,
                    IsChargedRoom = item.IsChargedRoom,
                    Deleted = false
                };

                return result;
            }
        }

        private IEnumerable<RoomDataImportCost> GetCosts()
        {
            using (var cmd = _conn.CreateCommand("dbo.Cost_Select"))
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("TableNameOrDescript", "RoomCost");
                cmd.Parameters.AddWithValue("ChargeDate", EndDate);

                if (RoomID > 0)
                    cmd.Parameters.AddWithValue("RecordID", RoomID);

                var dt = new DataTable();
                adap.Fill(dt);

                var result = dt.AsEnumerable().Select(x => new RoomDataImportCost
                {
                    CostID = x.Field<int>("CostID"),
                    ChargeTypeID = x.Field<int>("ChargeTypeID"),
                    TableNameOrDescript = x.Field<string>("TableNameOrDescript"),
                    RecordID = x.Field<int>("RecordID"),
                    AcctPer = x.Field<string>("AcctPer"),
                    AddVal = x.Field<double>("AddVal"),
                    MulVal = x.Field<double>("MulVal"),
                    EffDate = x.Field<DateTime>("EffDate"),
                }).ToList();

                return result;
            }
        }

        private IEnumerable<RoomDataImportRoom> GetRooms()
        {
            using (var cmd = _conn.CreateCommand("SELECT * FROM dbo.Room WHERE RoomID = ISNULL(@RoomID, RoomID) AND Active = 1", CommandType.Text))
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("TableNameOrDescript", "RoomCost");
                cmd.Parameters.AddWithValue("ChargeDate", EndDate);

                if (RoomID > 0)
                    cmd.Parameters.AddWithValue("RoomID", RoomID);
                else
                    cmd.Parameters.AddWithValue("RoomID", DBNull.Value);

                var dt = new DataTable();
                adap.Fill(dt);

                var result = dt.AsEnumerable().Select(x => new RoomDataImportRoom
                {
                    RoomID = x.Field<int>("RoomID"),
                    RoomName = x.Field<string>("Room"),
                    RoomDisplayName = x.Field<string>("DisplayName"),
                    PassbackRoom = x.Field<bool>("PassbackRoom"),
                    Billable = x.Field<bool>("Billable"),
                    IsChargedRoom = IsChargedRoom(x.Field<int>("RoomID")),
                    Active = x.Field<bool>("Active")
                }).ToList();

                return result;
            }
        }

        private bool IsChargedRoom(int roomId)
        {
            var cost = Costs.FirstOrDefault(x => x.RecordID == roomId);
            if (cost == null) return false;
            var result = cost.AcctPer != "None";
            return result;
        }

        private T ConvertTo<T>(object obj, T defval)
        {
            if (obj == null || obj == DBNull.Value)
                return defval;
            else
                return (T)obj;
        }

        public void Dispose()
        {
            _conn.Close();
            _conn.Dispose();
        }
    }

    public class RoomDataImportItem
    {
        public int RoomDataImportID { get; set; }
        public byte[] RID { get; set; }
        public int ClientID { get; set; }
        public string RoomName { get; set; }
        public DateTime EventDate { get; set; }
        public string EventDescription { get; set; }
        public int RoomID { get; set; }
        public string RoomDisplayName { get; set; }
        public bool PassbackRoom { get; set; }
        public bool Billable { get; set; }
        public bool IsChargedRoom { get; set; }
        public bool Deleted { get; set; }

        public bool IsAntipassbackError() => EventDescription.StartsWith("Antipassback");

        public override string ToString()
        {
            return $"{RoomName}:{ClientID}:{EventDescription}:{EventDate:yyyy-MM-dd HH:mm:ss}";
        }
    }

    public class RoomDataImportRoom
    {
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public string RoomDisplayName { get; set; }
        public bool PassbackRoom { get; set; }
        public bool Billable { get; set; }
        public bool IsChargedRoom { get; set; }
        public bool Active { get; set; }
    }

    public class RoomDataImportCost
    {
        public int CostID { get; set; }
        public int ChargeTypeID { get; set; }
        public string TableNameOrDescript { get; set; }
        public int RecordID { get; set; }
        public string AcctPer { get; set; }
        public double AddVal { get; set; }
        public double MulVal { get; set; }
        public DateTime EffDate { get; set; }
    }
}
