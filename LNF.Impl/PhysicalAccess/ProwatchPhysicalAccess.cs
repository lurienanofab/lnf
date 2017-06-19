using LNF.PhysicalAccess;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.PhysicalAccess
{
    public class ProwatchPhysicalAccess : IPhysicalAccessProvider
    {
        public IEnumerable<Badge> GetBadge(Client client = null)
        {
            using (var dba = ProwatchUtility.GetDBA())
            {
                string sql = @"
                    SELECT ID, BADGE_CLIENTID, BADGE_SSEL_UNAME, LNAME, FNAME, ISSUE_DATE, EXPIRE_DATE, EVENT_TIME, CARD_NO, EVENT_DESCRP, AREA_NAME, ALT_DESCRP 
                    FROM LNF.dbo.Badges
                    WHERE BADGE_CLIENTID = ISNULL(@ClientID, BADGE_CLIENTID)";
                int clientId = (client == null) ? 0 : client.ClientID;
                dba.SelectCommand.AddParameter("@ClientID", clientId).CommandTypeText();
                DataTable dt = dba.FillDataTable(sql);
                IList<Badge> result = dt.AsEnumerable().Select(ProwatchUtility.CreateBadge).ToList();
                return result;
            }
        }

        public IEnumerable<Card> GetCards(Badge badge = null)
        {
            using (var dba = ProwatchUtility.GetDBA())
            {
                string sql = @"
                    SELECT ID, BADGE_CLIENTID, BADGE_SSEL_UNAME, LNAME, FNAME, CARDNO, LAST_ACC, CARD_ISSUE_DATE, CARD_EXPIRE_DATE, BADGE_ISSUE_DATE, BADGE_EXPIRE_DATE, STAT_COD
                    FROM LNF.dbo.Cards
                    WHERE BADGE_CLIENTID = ISNULL(@ClientID, BADGE_CLIENTID)";
                int clientId = (badge == null) ? 0 : badge.ClientID;
                dba.SelectCommand.AddParameterIf("@ClientID", clientId > 0, clientId).CommandTypeText();
                IList<Card> result = dba.FillDataTable(sql).AsEnumerable().Select(ProwatchUtility.CreateCard).ToList();
                return result;
            }
        }

        public IEnumerable<LNF.PhysicalAccess.Area> GetAreas()
        {
            using (var dba = ProwatchUtility.GetDBA())
            {
                string sql = "SELECT ID, AREA_NAME FROM LNF.dbo.Areas";
                DataTable dt = dba.CommandTypeText().FillDataTable(sql);
                IList<LNF.PhysicalAccess.Area> result = dt.AsEnumerable().Select(ProwatchUtility.CreateArea).ToList();
                return result;
            }
        }

        public IEnumerable<Badge> CurrentlyInArea()
        {
            using (var dba = ProwatchUtility.GetDBA())
            {
                string sql = "SELECT ID, BADGE_CLIENTID, BADGE_SSEL_UNAME, LNAME, FNAME, ISSUE_DATE, EXPIRE_DATE, EVENT_TIME, CARD_NO, EVENT_DESCRP, AREA_NAME, ALT_DESCRP FROM LNF.dbo.UsersInLab";
                DataTable dt = dba.CommandTypeText().FillDataTable(sql);
                IList<Badge> result = new List<Badge>();
                foreach (DataRow dr in dt.Rows)
                    result.Add(ProwatchUtility.CreateBadge(dr));
                return result;
            }
        }

        //Selects clients whose cards will expire before the cutoff date
        public IEnumerable<Card> ExpiringCards(DateTime cutoff)
        {
            IEnumerable<Card> list = GetCards();
            IList<Card> result = list.Where(x => (x.CardExpireDate < cutoff || x.BadgeExpireDate < cutoff) && x.Status == Status.Active).ToList();
            return result;
        }

        public DataTable RawData(DateTime sd, DateTime ed, int clientId = 0, string roomName = null)
        {
            using (var dba = ProwatchUtility.GetDBA())
            {
                string sql = "SELECT * FROM LNF.dbo.RawData WHERE ISNULL(@ClientID, BADGE_CLIENTID) = BADGE_CLIENTID AND ISNULL(@AreaName, LOGDEVADESCRP) = LOGDEVADESCRP AND (EVNT_DAT >= @StartDate AND EVNT_DAT < @EndDate)";

                object cid = (clientId == 0) ? DBNull.Value : (object)clientId;
                object areaName = string.IsNullOrEmpty(roomName) ? DBNull.Value : (object)roomName;

                DataTable dt = dba
                    .CommandTypeText()
                    .AddParameter("@ClientID", cid)
                    .AddParameter("@AreaName", areaName)
                    .AddParameter("@StartDate", sd)
                    .AddParameter("@EndDate", ed)
                    .FillDataTable(sql);

                return dt;
            }
        }

        public bool IsPassbackRoom(Room room)
        {
            if (room == null)
                return false;
            else
                return room.PassbackRoom;
        }
        
        public IEnumerable<Event> GetEvents(DateTime startDate, DateTime endDate, Client client = null, Room room = null)
        {
            var raw = RawData(startDate, endDate, client.ClientID, room.RoomName);

            var result = ProwatchUtility.CreateEvents(raw);

            //remove rows with bad data
            string[] badEvents = { "Local Grant", "Host Grant", "Local Grant - APB Error - Used", "Local Grant - Door not used" };
            result.RemoveAll(x => IsPassbackRoom(x.Room) && badEvents.Contains(x.EventDescription));

            //fix antipassback issues
            int pbcutoff = 90;
            Event[] pbvios = result.Where(x => x.EventDescription.StartsWith("Antipassback")).ToArray();
            foreach (Event pbe in pbvios)
            {
                int count = result.Where(x => x.Client == pbe.Client && x.Room == pbe.Room && !x.IsAntipassbackError() && Math.Abs((x.EventDateTime - pbe.EventDateTime).TotalSeconds) <= pbcutoff).Count();
                if (count > 0)
                    result.RemoveAll(x => x.Client == pbe.Client && x.Room == pbe.Room && x.IsAntipassbackError() && x.EventDateTime == pbe.EventDateTime);
            }

            //the min event for each client/room should always be an IN, so get the events for each client/room where the min event in an OUT and find the IN
            var minOUT = result.Where(x => x.EventType == EventType.Out).Join(
                result.Where(x => IsPassbackRoom(x.Room)).GroupBy(x => new { x.Client, x.Room }).Select(x => new { x.Key.Client, x.Key.Room, EventDateTime = x.Min(g => g.EventDateTime) }),
                o => new { o.Client, o.Room, o.EventDateTime },
                i => new { i.Client, i.Room, i.EventDateTime },
                (o, i) => o).ToArray();

            if (minOUT.Length > 0)
                result.AddRange(minOUT.Select(x => GetPreviousIn(x, startDate)));

            //the max event for each client/room should always be an OUT, so get the events for each client/room where the max event in an IN and find the OUT
            var maxIN = result.Where(x => x.EventType == EventType.In).Join(
                result.Where(x => IsPassbackRoom(x.Room)).GroupBy(x => new { x.Client, x.Room }).Select(x => new { x.Key.Client, x.Key.Room, EventDateTime = x.Max(g => g.EventDateTime) }),
                 o => new { o.Client, o.Room, o.EventDateTime },
                 i => new { i.Client, i.Room, i.EventDateTime },
                 (o, i) => o).ToArray();

            if (maxIN.Length > 0)
                result.AddRange(maxIN.Select(x => GetNextOut(x, endDate)));

            return result
                .OrderBy(x => x.Client.ClientID)
                .ThenBy(x => x.Room.RoomID)
                .ThenBy(x => x.EventDateTime)
                .ToArray();
        }

        private Event GetPreviousIn(Event e, DateTime startDate)
        {
            using (var dba = ProwatchUtility.GetDBA())
            {
                string eventDescription = "Local Grant - IN";

                string sql = "SELECT TOP 1 * FROM LNF.dbo.RawData WHERE BADGE_CLIENTID = @ClientID AND LOGDEVADESCRP = @AreaName AND EVNT_DAT < @StartDate AND EVNT_DESCRP = @EventDescription ORDER BY EVNT_DAT DESC";

                var dt = dba
                    .CommandTypeText()
                    .ApplyParameters(new { ClientID = e.Client.ClientID, AreaName = e.Room.RoomName, StartDate = startDate, EventDescription = eventDescription })
                    .FillDataTable(sql);

                if (dt == null || dt.Rows.Count == 0)
                {
                    //for lack of a better idea, when a fake record is generated put 0x01 at the beginning of the ID so that
                    //it will still be unique but also identifiable as the generated record for the given OUT event (0x01 for IN, 0x02 for OUT)
                    var bytes = ProwatchUtility.StringToBytes(e.ID).ToList();
                    bytes.Insert(0, 0x01);

                    return new Event()
                    {
                        Client = e.Client,
                        Room = e.Room,
                        EventType = EventType.In,
                        EventDescription = eventDescription,
                        EventDateTime = startDate,
                        UserName = e.UserName,
                        LastName = e.LastName,
                        FirstName = e.FirstName,
                        DeviceID = e.DeviceID,
                        ID = ProwatchUtility.BytesToString(bytes),
                        DeviceDescription = e.DeviceDescription,
                        CardExpireDate = e.CardExpireDate,
                        CardIssueDate = e.CardIssueDate,
                        CardNumber = e.CardNumber,
                        CardStatus = e.CardStatus
                    };
                }
                else
                {
                    return ProwatchUtility.CreateEvents(dt).First();
                }
            }
        }

        private Event GetNextOut(Event e, DateTime endDate)
        {
            using (var dba = ProwatchUtility.GetDBA())
            {
                string eventDescription = "Local Grant - OUT";

                string sql = "SELECT TOP 1 * FROM LNF.dbo.RawData WHERE BADGE_CLIENTID = @ClientID AND LOGDEVADESCRP = @AreaName AND EVNT_DAT > @EndDate AND EVNT_DESCRP = @EventDescription ORDER BY EVNT_DAT ASC";

                var dt = dba
                    .CommandTypeText()
                    .ApplyParameters(new { ClientID = e.Client.ClientID, AreaName = e.Room.RoomName, EndDate = endDate, EventDescription = eventDescription })
                    .FillDataTable(sql);

                if (dt == null || dt.Rows.Count == 0)
                {
                    //for lack of a better idea, when a fake record is generated put 0x02 at the beginning of the ID so that
                    //it will still be unique but also identifiable as the generated record for the given IN event (0x01 for IN, 0x02 for OUT)
                    var bytes = ProwatchUtility.StringToBytes(e.ID).ToList();
                    bytes.Insert(0, 0x02);

                    return new Event()
                    {
                        Client = e.Client,
                        Room = e.Room,
                        EventType = EventType.Out,
                        EventDescription = eventDescription,
                        EventDateTime = endDate,
                        UserName = e.UserName,
                        LastName = e.LastName,
                        FirstName = e.FirstName,
                        DeviceID = e.DeviceID,
                        ID = ProwatchUtility.BytesToString(bytes),
                        DeviceDescription = e.DeviceDescription,
                        CardExpireDate = e.CardExpireDate,
                        CardIssueDate = e.CardIssueDate,
                        CardNumber = e.CardNumber,
                        CardStatus = e.CardStatus
                    };
                }
                else
                {
                    return ProwatchUtility.CreateEvents(dt).First();
                }
            }
        }

        public bool AllowReenable(int clientId, int dayCount)
        {
            using (var dba = ProwatchUtility.GetDBA())
            {
                string sql = "SELECT CONVERT(bit, CASE WHEN DATEDIFF(DAY, EXPIRE_DATE, @Date) > @DayCount THEN 0 ELSE 1 END) FROM LNF.dbo.Badges WHERE BADGE_CLIENTID = @ClientID";
                bool result = dba.CommandTypeText().ApplyParameters(new { Date = DateTime.Now, DayCount = dayCount, ClientID = clientId }).ExecuteScalar<bool>(sql);
                return result;
            }
        }

        //Selects those clients who have had passback violations during the period
        public int[] CheckPassbackViolations(DateTime startDate, DateTime endDate)
        {
            using (var dba = ProwatchUtility.GetDBA())
            {
                string sql = "SELECT DISTINCT bv.BADGE_CLIENTID FROM PWNT.dbo.EV_LOG elog, PWNT.dbo.BADGE_V bv WHERE bv.ID = elog.BADGENO AND elog.EVNT_DAT >= @StartDate AND elog.EVNT_DAT < @EndDate AND elog.EVNT_DESCRP LIKE 'Antipassback%' ORDER BY bv.BADGE_CLIENTID";
                DataTable dt = dba.CommandTypeText().ApplyParameters(new { StartDate = startDate, EndDate = endDate }).FillDataTable(sql);
                int[] result = dt.AsEnumerable().Select(x => Convert.ToInt32(x["BADGE_CLIENTID"])).ToArray();
                return result;
            }
        }

        public void AddClient(Client c)
        {
            using (var dba = ProwatchUtility.GetDBA())
            {
                dba.ApplyParameters(new
                {
                    Action = "AddClient",
                    UserName = c.UserName,
                    FName = c.FName,
                    MName = c.MName,
                    LName = c.LName,
                    ClientID = c.ClientID
                }).ExecuteNonQuery("LNF.dbo.ClientUpdate");
            }
        }

        public void EnableAccess(Client c)
        {
            using (var dba = ProwatchUtility.GetDBA())
                dba.ApplyParameters(new { Action = "EnableAccess", ClientID = c.ClientID }).ExecuteNonQuery("LNF.dbo.ClientUpdate");
        }

        public void DisableAccess(Client c)
        {
            using (var dba = ProwatchUtility.GetDBA())
                dba.ApplyParameters(new { Action = "DisableAccess", ClientID = c.ClientID }).ExecuteNonQuery("LNF.dbo.ClientUpdate");
        }

        public IEnumerable<RoomDataRaw> GetRoomData(DateTime sd, DateTime ed, int clientId, string roomName)
        {
            using (var dba = ProwatchUtility.GetDBA())
            {
                var dt = RawData(sd, ed, clientId, roomName);
                throw new NotImplementedException();
            }   
        }

        public void Dispose()
        {

        }
    }
}
