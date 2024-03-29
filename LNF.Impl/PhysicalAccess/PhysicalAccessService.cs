﻿using LNF.Data;
using LNF.PhysicalAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace LNF.Impl.PhysicalAccess
{
    public class PhysicalAccessService : IPhysicalAccessService
    {
        private IEnumerable<IRoom> _rooms = null;

        public int AddClient(AddClientRequest request)
        {
            return Repository.Prowatch.ExecuteNonQuery("LNF.dbo.ClientUpdate", new Dictionary<string, object>
            {
                ["Action"] = "AddClient",
                ["UserName"] = request.UserName,
                ["FName"] = request.FName,
                ["MName"] = request.MName,
                ["LName"] = request.LName,
                ["ClientID"] = request.ClientID
            }, CommandType.StoredProcedure);
        }

        public int DisableAccess(UpdateClientRequest request)
        {
            return Repository.Prowatch.ExecuteNonQuery("LNF.dbo.ClientUpdate", new Dictionary<string, object>
            {
                ["Action"] = "DisableAccess",
                ["ClientID"] = request.ClientID,
                ["ExpireOn"] = Utility.DBNullIf(request.ExpireOn)
            }, CommandType.StoredProcedure);
        }

        public int EnableAccess(UpdateClientRequest request)
        {
            return Repository.Prowatch.ExecuteNonQuery("LNF.dbo.ClientUpdate", new Dictionary<string, object>
            {
                ["Action"] = "EnableAccess",
                ["ClientID"] = request.ClientID,
                ["ExpireOn"] = Utility.DBNullIf(request.ExpireOn)
            }, CommandType.StoredProcedure);
        }

        public Event FindNextOut(FindNextOutRequest request)
        {
            var rooms = GetRooms();
            var sd = request.EndDate.AddMonths(-1);
            var ed = request.EndDate;
            var e = GetEvent(request.EventID, sd, ed, rooms);
            return FindNextOut(e, request.EndDate, rooms);
        }

        private Event FindNextOut(Event e, DateTime ed, IEnumerable<IRoom> rooms)
        {
            string eventDescription = "Local Grant - OUT";

            var dt = SelectRawData(ed.AddMonths(-1), ed, e.ClientID, e.DeviceDescription, eventDescription, null);

            if (dt == null || dt.Rows.Count == 0)
            {
                //for lack of a better idea, when a fake record is generated put 0x02 at the beginning of the ID so that
                //it will still be unique but also identifiable as the generated record for the given IN event (0x01 for IN, 0x02 for OUT)
                var bytes = Utility.StringToBytes(e.ID).ToList();
                bytes.Insert(0, 0x02);

                return new Event()
                {
                    ClientID = e.ClientID,
                    RoomID = e.RoomID,
                    EventType = EventType.Out,
                    EventDescription = eventDescription,
                    EventDateTime = ed,
                    UserName = e.UserName,
                    LastName = e.LastName,
                    FirstName = e.FirstName,
                    DeviceID = e.DeviceID,
                    ID = Utility.BytesToString(bytes),
                    DeviceDescription = e.DeviceDescription,
                    CardExpireDate = e.CardExpireDate,
                    CardIssueDate = e.CardIssueDate,
                    CardNumber = e.CardNumber,
                    CardStatus = e.CardStatus
                };
            }
            else
            {
                return Utility.CreateEvents(dt, rooms).First();
            }
        }

        public Event FindPreviousIn(FindPreviousInRequest request)
        {
            var rooms = GetRooms();
            var sd = request.StartDate;
            var ed = request.StartDate.AddMonths(1);
            var e = GetEvent(request.EventID, sd, ed, rooms);
            return FindPreviousIn(e, request.StartDate, rooms);
        }

        private Event FindPreviousIn(Event e, DateTime sd, IEnumerable<IRoom> rooms)
        {
            string eventDescription = "Local Grant - IN";

            var dt = SelectRawData(sd, sd.AddMonths(1), e.ClientID, e.DeviceDescription, eventDescription, null);

            if (dt == null || dt.Rows.Count == 0)
            {
                //for lack of a better idea, when a fake record is generated put 0x01 at the beginning of the ID so that
                //it will still be unique but also identifiable as the generated record for the given OUT event (0x01 for IN, 0x02 for OUT)
                var bytes = Utility.StringToBytes(e.ID).ToList();
                bytes.Insert(0, 0x01);

                return new Event()
                {
                    ClientID = e.ClientID,
                    RoomID = e.RoomID,
                    EventType = EventType.In,
                    EventDescription = eventDescription,
                    EventDateTime = sd,
                    UserName = e.UserName,
                    LastName = e.LastName,
                    FirstName = e.FirstName,
                    DeviceID = e.DeviceID,
                    ID = Utility.BytesToString(bytes),
                    DeviceDescription = e.DeviceDescription,
                    CardExpireDate = e.CardExpireDate,
                    CardIssueDate = e.CardIssueDate,
                    CardNumber = e.CardNumber,
                    CardStatus = e.CardStatus
                };
            }
            else
            {
                return Utility.CreateEvents(dt, rooms).First();
            }
        }


        public bool GetAllowReenable(int clientId, int days)
        {
            string sql = "SELECT CONVERT(bit, CASE WHEN DATEDIFF(DAY, EXPIRE_DATE, @Date) > @DayCount THEN 0 ELSE 1 END) FROM LNF.dbo.Badges WHERE BADGE_CLIENTID = @ClientID";

            bool result = Convert.ToBoolean(Repository.Prowatch.ExecuteScalar(sql, new Dictionary<string, object>
            {
                ["Date"] = DateTime.Now,
                ["DayCount"] = days,
                ["ClientID"] = clientId
            }));

            return result;
        }

        public IEnumerable<Area> GetAreas()
        {
            string sql = "SELECT ID, AREA_NAME FROM LNF.dbo.Areas";

            var dt = Repository.Prowatch.FillDataTable(sql);

            IList<Area> result = Utility.CreateAreas(dt);

            return result;
        }

        public IEnumerable<Area> GetAreas(int[] areaIds)
        {
            string list = string.Join(",", areaIds);
            string sql = $"SELECT ID, AREA_NAME FROM LNF.dbo.Areas WHERE ID IN ({list})";

            var dt = Repository.Prowatch.FillDataTable(sql);

            IList<Area> result = Utility.CreateAreas(dt);

            return result;
        }

        public IEnumerable<Badge> GetBadge(int clientId = 0)
        {
            string sql = "SELECT ID, BADGE_CLIENTID"
                + ", BADGE_SSEL_UNAME, LNAME, FNAME"
                + ", ISSUE_DATE, EXPIRE_DATE, EVENT_TIME"
                + ", CARD_NO, EVENT_DESCRP"
                + ", AREA_NAME, ALT_DESCRP "
                + " FROM LNF.dbo.Badges "
                + " WHERE BADGE_CLIENTID = ISNULL(@ClientID, BADGE_CLIENTID)";

            var dt = Repository.Prowatch.FillDataTable(sql, new Dictionary<string, object>
            {
                ["ClientID"] = Utility.DBNullIf(clientId, clientId == 0)
            });

            IList<Badge> result = Utility.CreateBadges(dt);

            return result;
        }

        public IEnumerable<Card> GetCards(int clientId = 0)
        {
            string sql = "SELECT ID, BADGE_CLIENTID"
                    + ", BADGE_SSEL_UNAME, LNAME, FNAME"
                    + ", CARDNO, LAST_ACC, CARD_ISSUE_DATE, CARD_EXPIRE_DATE"
                    + ", BADGE_ISSUE_DATE, BADGE_EXPIRE_DATE, STAT_COD "
                    + " FROM LNF.dbo.Cards "
                    + " WHERE BADGE_CLIENTID = ISNULL(@ClientID, BADGE_CLIENTID)";

            var dt = Repository.Prowatch.FillDataTable(sql, new Dictionary<string, object>
            {
                ["ClientID"] = Utility.DBNullIf(clientId, clientId == 0)
            });

            IList<Card> result = Utility.CreateCards(dt);

            return result;
        }

        public Card GetCard(string cardnum)
        {
            string sql = "SELECT ID, BADGE_CLIENTID"
                    + ", BADGE_SSEL_UNAME, LNAME, FNAME"
                    + ", CARDNO, LAST_ACC, CARD_ISSUE_DATE, CARD_EXPIRE_DATE"
                    + ", BADGE_ISSUE_DATE, BADGE_EXPIRE_DATE, STAT_COD"
                    + " FROM LNF.dbo.Cards"
                    + " WHERE CARDNO = @CARDNO";

            var dt = Repository.Prowatch.FillDataTable(sql, new Dictionary<string, object>
            {
                ["CARDNO"] = cardnum
            });

            IList<Card> list = Utility.CreateCards(dt);
            var result = list.First();

            return result;
        }

        public IEnumerable<Badge> GetCurrentlyInArea(string alias)
        {
            return Repository.Prowatch.GetCurrentlyInArea(alias);
        }

        public IEnumerable<BadgeInArea> GetBadgeInAreas(string alias)
        {
            return Repository.Prowatch.GetBadgeInAreas(alias);
        }

        public IEnumerable<Event> GetEvents(DateTime sd, DateTime ed, int clientId = 0, int roomId = 0)
        {
            var rooms = GetRooms();

            var result = GetRawData(sd, ed, clientId, roomId, rooms).ToList();

            //remove rows with bad data
            string[] badEvents = { "Local Grant", "Host Grant", "Local Grant - APB Error - Used", "Local Grant - Door not used" };
            result.RemoveAll(x => Utility.IsPassbackRoom(x, rooms) && badEvents.Contains(x.EventDescription));

            //fix antipassback issues
            int pbcutoff = 90;
            Event[] pbvios = result.Where(x => x.EventDescription.StartsWith("Antipassback")).ToArray();
            foreach (Event pbe in pbvios)
            {
                int count = result.Where(x => x.ClientID == pbe.ClientID && x.DeviceDescription == pbe.DeviceDescription && !x.IsAntipassbackError() && Math.Abs((x.EventDateTime - pbe.EventDateTime).TotalSeconds) <= pbcutoff).Count();
                if (count > 0)
                    result.RemoveAll(x => x.ClientID == pbe.ClientID && x.DeviceDescription == pbe.DeviceDescription && x.IsAntipassbackError() && x.EventDateTime == pbe.EventDateTime);
            }

            //the min event for each client/room should always be an IN, so get the events for each client/room where the min event in an OUT and find the IN
            var minOUT = result.Where(x => x.EventType == EventType.Out).Join(
                result.Where(x => Utility.IsPassbackRoom(x, rooms)).GroupBy(x => new { x.ClientID, x.RoomID }).Select(x => new { x.Key.ClientID, x.Key.RoomID, EventDateTime = x.Min(g => g.EventDateTime) }),
                o => new { o.ClientID, o.RoomID, o.EventDateTime },
                i => new { i.ClientID, i.RoomID, i.EventDateTime },
                (o, i) => o).ToArray();

            if (minOUT.Length > 0)
                result.AddRange(minOUT.Select(x => FindPreviousIn(x, sd, rooms)));

            //the max event for each client/room should always be an OUT, so get the events for each client/room where the max event in an IN and find the OUT
            var maxIN = result.Where(x => x.EventType == EventType.In).Join(
                result.Where(x => Utility.IsPassbackRoom(x, rooms)).GroupBy(x => new { x.ClientID, x.RoomID }).Select(x => new { x.Key.ClientID, x.Key.RoomID, EventDateTime = x.Max(g => g.EventDateTime) }),
                 o => new { o.ClientID, o.RoomID, o.EventDateTime },
                 i => new { i.ClientID, i.RoomID, i.EventDateTime },
                 (o, i) => o).ToArray();

            if (maxIN.Length > 0)
                result.AddRange(maxIN.Select(x => FindNextOut(x, ed, rooms)));

            return result
                .OrderBy(x => x.ClientID)
                .ThenBy(x => x.RoomID)
                .ThenBy(x => x.EventDateTime)
                .ToArray();
        }

        public IEnumerable<Event> GetRawData(DateTime sd, DateTime ed, int clientId, int roomId)
        {
            var rooms = GetRooms();
            return GetRawData(sd, ed, clientId, roomId, rooms);
        }

        private IEnumerable<Event> GetRawData(DateTime sd, DateTime ed, int clientId, int roomId, IEnumerable<IRoom> rooms)
        {
            var areaName = Utility.GetRoomName(roomId, rooms);
            var dt = SelectRawData(sd, ed, clientId, areaName, null, null);
            var result = Utility.CreateEvents(dt, rooms);
            return result;
        }

        public IEnumerable<Card> GetExpiringCards(DateTime cutoff)
        {
            IEnumerable<Card> list = GetCards();
            IList<Card> result = list.Where(x => (x.CardExpireDate < cutoff || x.BadgeExpireDate < cutoff) && x.Status == Status.Active).ToList();
            return result;
        }

        public int[] GetPassbackViolations(DateTime sd, DateTime ed)
        {
            string sql = "SELECT DISTINCT bv.BADGE_CLIENTID FROM PWNT.dbo.EV_LOG elog, PWNT.dbo.BADGE_V bv WHERE bv.ID = elog.BADGENO AND elog.EVNT_DAT >= @StartDate AND elog.EVNT_DAT < @EndDate AND elog.EVNT_DESCRP LIKE 'Antipassback%' ORDER BY bv.BADGE_CLIENTID";

            var dt = Repository.Prowatch.FillDataTable(sql, new Dictionary<string, object>
            {
                ["StartDate"] = sd,
                ["EndDate"] = ed
            });

            int[] result = dt.AsEnumerable().Select(x => Convert.ToInt32(x["BADGE_CLIENTID"])).ToArray();

            return result;
        }

        public Event GetEvent(string eventId, DateTime sd, DateTime ed, IEnumerable<IRoom> rooms)
        {
            // eventId should be a hex 18 byte hex number e.g. 0071C975F91B61744AE5A16D8FED80D6CE75 (no leading 0x)

            var rid = eventId.ToUpper();

            if (!Regex.IsMatch(rid, "^[0-9A-F]{36}$"))
                throw new Exception("Invalid eventId. Expecting an 18 byte hex string: two characters per byte for a total length of 36 characters.");

            var dt = SelectRawData(sd, ed, 0, null, null, rid);

            var events = Utility.CreateEvents(dt, rooms);

            var result = events.FirstOrDefault();

            if (result == null)
                throw new Exception($"Cannot find event with EventID: {eventId}");

            return result;
        }

        private IEnumerable<IRoom> GetRooms()
        {
            if (_rooms == null)
            {
                var dt = Repository.LNF.FillDataTable("SELECT * FROM dbo.Room WHERE Active = 1");
                _rooms = Utility.CreateRooms(dt);
            }

            return _rooms;
        }

        private DataTable SelectRawData(DateTime sd, DateTime ed, int clientId, string areaName, string eventDesc, string rid)
        {
            return Repository.Prowatch.FillDataTable("LNF.dbo.EventLog_Select", new Dictionary<string, object>
            {
                ["Action"] = "RawData",
                ["StartDate"] = sd,
                ["EndDate"] = ed,
                ["ClientID"] = Utility.DBNullIf(clientId, clientId == 0),
                ["AreaName"] = Utility.DBNullIf(areaName, string.IsNullOrEmpty(areaName)),
                ["EventDescription"] = Utility.DBNullIf(eventDesc, string.IsNullOrEmpty(eventDesc)),
                ["RID"] = Utility.DBNullIf(rid, string.IsNullOrEmpty(rid))
            }, CommandType.StoredProcedure);
        }
    }
}
