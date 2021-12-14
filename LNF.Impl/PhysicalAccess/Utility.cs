using LNF.CommonTools;
using LNF.Data;
using LNF.PhysicalAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LNF.Impl.PhysicalAccess
{
    public static class Utility
    {
        private static readonly IDictionary<string, string> ExpiredCardFix = new Dictionary<string, string>()
        {
            {"0070DFA7ABB31C9D47AD9924AEABE35C65C5", "Local Grant - IN"},   //True for both Clean Room and Wet Chemistry
            {"007061B296E227C14301AB77EF94BFBA697B", "Local Grant - OUT"},  //True for both Clean Room and Wet Chemistry
            {"0070D40949A88D5F11D4A45600508BC86902", "Local Grant"},        //6.5 - 14C7
            {"0070A1F0B78F41BE4436A8AE7D7FEFAE6B0A", "Local Grant"},        //Store
            {"007042353239464538332D464638382D3436", "Local Grant"}         //Forrest Lab,Wet Chem, And Service Aisle
        };

        private static readonly IDictionary<string, EventType> EventTypeLookup = new Dictionary<string, EventType>()
        {
            {"Local Grant", EventType.Grant},
            {"Local Grant - Door not used", EventType.Grant},
            {"Local Grant - IN", EventType.In},
            {"Local Grant - OUT", EventType.Out},
            {"Local Grant - Door not used - IN", EventType.In},
            {"Local Grant - Door not used - OUT", EventType.Out},
            {"Antipassback error - IN", EventType.AntipassbackErrorIn},
            {"Antipassback error - OUT", EventType.AntipassbackErrorOut}
        };

        public static int? GetCardNumber(DataRow dr)
        {
            if (dr.Table.Columns.Contains("CARD_NO"))
            {
                object value = dr["CARD_NO"];

                if (value == DBNull.Value)
                    return null;

                if (!int.TryParse(value.ToString(), out int result))
                    return null;

                return result;
            }

            return null;
        }

        public static IList<Badge> CreateBadges(DataTable dt)
        {
            var result = new List<Badge>();

            if (dt == null) return result;
            if (dt.Rows.Count == 0) return result;

            foreach (DataRow dr in dt.Rows)
            {
                result.Add(new Badge()
                {
                    ID = BytesToString(dr["ID"]),
                    ClientID = dr.Field<int>("BADGE_CLIENTID"),
                    UserName = dr.Field<string>("BADGE_SSEL_UNAME"),
                    LastName = dr.Field<string>("LNAME"),
                    FirstName = dr.Field<string>("FNAME"),
                    IssueDate = FixEventDate(dr.Field<DateTime>("ISSUE_DATE")).Value,
                    ExpireDate = FixEventDate(dr.Field<DateTime>("EXPIRE_DATE")).Value,
                    CurrentAccessTime = FixEventDate(dr.Field<DateTime?>("EVENT_TIME")),
                    CurrentCardNumber = GetCardNumber(dr),
                    CurrentEventDescription = dr.Field<string>("EVENT_DESCRP"),
                    CurrentAreaName = dr.Field<string>("AREA_NAME"),
                    AltDescription = dr.Field<string>("ALT_DESCRP")
                });
            }

            return result;
        }

        public static IList<Card> CreateCards(DataTable dt)
        {
            var result = new List<Card>();

            if (dt == null) return result;
            if (dt.Rows.Count == 0) return result;

            foreach (DataRow dr in dt.Rows)
            {
                string id = BytesToString(dr["ID"]);
                int clientId = dr.Field<int>("BADGE_CLIENTID");
                string userName = dr.Field<string>("BADGE_SSEL_UNAME");
                string lastName = dr.Field<string>("LNAME");
                string firstName = dr.Field<string>("FNAME");
                int cardNumber = dr["CARDNO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CARDNO"]);
                DateTime? lastAccess = dr.Field<DateTime?>("LAST_ACC");
                DateTime cardIssueDate = dr.Field<DateTime>("CARD_ISSUE_DATE");
                DateTime cardExpireDate = dr.Field<DateTime>("CARD_EXPIRE_DATE");
                DateTime badgeIssueDate = dr.Field<DateTime>("BADGE_ISSUE_DATE");
                DateTime badgeExpireDate = dr.Field<DateTime>("BADGE_EXPIRE_DATE");
                Status cardStatus = GetCardStatus(dr.Field<string>("STAT_COD"));
                
                result.Add(new Card()
                {
                    ID = id,
                    ClientID = clientId,
                    UserName = userName,
                    LastName = lastName,
                    FirstName = firstName,
                    Number = cardNumber,
                    LastAccess = lastAccess,
                    CardIssueDate = cardIssueDate,
                    CardExpireDate = cardExpireDate,
                    BadgeIssueDate = badgeIssueDate,
                    BadgeExpireDate = badgeExpireDate,
                    Status = cardStatus
                });
            }

            return result;
        }

        public static IList<Event> CreateEvents(DataTable raw, IEnumerable<IRoom> rooms)
        {
            var result = new List<Event>();

            if (raw == null) return result;

            foreach (DataRow dr in raw.Rows)
            {
                string eventId = BytesToString(dr["RID"]);
                string deviceId = BytesToString(dr["LOGDEVDTLID"]);
                string eventDesc = GetEventDescription(deviceId, Convert.ToString(dr["EVNT_DESCRP"]));
                string deviceDesc = Convert.ToString(dr["LOGDEVADESCRP"]);
                int cardnum = Convert.ToInt32(dr["CARDNO"]); //this is a string padded to 32 length with trailing null chars
                int roomId = Convert.ToInt32(rooms.FirstOrDefault(x => x.RoomName == deviceDesc)?.RoomID);

                result.Add(new Event()
                {
                    ID = eventId,
                    DeviceID = deviceId,
                    ClientID = dr.Field<int>("BADGE_CLIENTID"),
                    RoomID = roomId,
                    EventType = GetEventType(eventDesc),
                    UserName = dr["BADGE_SSEL_UNAME"].ToString(),
                    LastName = dr["LNAME"].ToString(),
                    FirstName = dr["FNAME"].ToString(),
                    EventDateTime = dr.Field<DateTime>("EVNT_DAT"),
                    EventDescription = eventDesc,
                    DeviceDescription = deviceDesc,
                    CardNumber = cardnum,
                    CardStatus = GetCardStatus(dr.Field<string>("STAT_COD")),
                    CardIssueDate = dr.Field<DateTime>("ISSUE_DAT"),
                    CardExpireDate = dr.Field<DateTime>("EXPIRE_DAT")
                });
            }

            return result;
        }

        public static List<Event> CreateEvents(DataTable raw)
        {
            var result = new List<Event>();

            foreach (DataRow dr in raw.Rows)
            {
                string eventId = BytesToString(dr["RID"]);
                string deviceId = BytesToString(dr["LOGDEVDTLID"]);
                string eventDesc = GetEventDescription(deviceId, dr["EVNT_DESCRP"].ToString());
                string deviceDesc = dr["LOGDEVADESCRP"].ToString();
                int cardnum = Convert.ToInt32(dr["CARDNO"]); //this is a string padded to 32 length with trailing null chars

                result.Add(new Event()
                {
                    ID = eventId,
                    DeviceID = deviceId,
                    ClientID = dr.Field<int>("BADGE_CLIENTID"),
                    EventType = GetEventType(eventDesc),
                    UserName = dr.Field<string>("BADGE_SSEL_UNAME"),
                    LastName = dr.Field<string>("LNAME"),
                    FirstName = dr.Field<string>("FNAME"),
                    EventDateTime = dr.Field<DateTime>("EVNT_DAT"),
                    EventDescription = eventDesc,
                    DeviceDescription = deviceDesc,
                    CardNumber = cardnum,
                    CardStatus = GetCardStatus(dr.Field<string>("STAT_COD")),
                    CardIssueDate = dr.Field<DateTime>("ISSUE_DAT"),
                    CardExpireDate = dr.Field<DateTime>("EXPIRE_DAT")
                });
            }

            return result;
        }

        public static string GetRoomName(int roomId, IEnumerable<IRoom> rooms)
        {
            var r = rooms.FirstOrDefault(x => x.RoomID == roomId);
            var result = r?.RoomName;
            return result;
        }

        public static IList<Area> CreateAreas(DataTable dt)
        {
            var result = new List<Area>();

            if (dt == null) return result;
            if (dt.Rows.Count == 0) return result;

            foreach (DataRow dr in dt.Rows)
            {
                result.Add(new Area()
                {
                    ID = BytesToString(dr["ID"]),
                    Name = dr.Field<string>("AREA_NAME")
                });
            }

            return result;
        }

        public static IEnumerable<IRoom> CreateRooms(DataTable dt)
        {
            var result = new List<IRoom>();

            if (dt == null) return result;
            if (dt.Rows.Count == 0) return result;

            foreach (DataRow dr in dt.Rows)
            {
                result.Add(new RoomItem()
                {
                    RoomID = dr.Field<int>("RoomID"),
                    ParentID = dr.Field<int?>("ParentID"),
                    RoomName = dr.Field<string>("Room"),
                    RoomDisplayName = dr.Field<string>("DisplayName"),
                    PassbackRoom = dr.Field<bool>("PassbackRoom"),
                    Billable = dr.Field<bool>("Billable"),
                    ApportionDailyFee = dr.Field<bool>("ApportionDailyFee"),
                    ApportionEntryFee = dr.Field<bool>("ApportionEntryFee"),
                    Active = dr.Field<bool>("Active")
                });
            }

            return result;
        }

        public static Status GetCardStatus(string code)
        {
            switch (code)
            {
                case "A":
                    return Status.Active;
                case "D":
                    return Status.Disabled;
                case "L":
                    return Status.Lost;
                case "S":
                    return Status.Stolen;
                case "T":
                    return Status.Terminated;
                case "U":
                    return Status.Unaccounted;
                case "V":
                    return Status.Void;
                case "X":
                    return Status.Expired;
                case "O":
                    return Status.AutoDisable;
                default:
                    return Status.Unknown;
            }
        }

        public static string GetAreaName(string alias)
        {
            switch (alias)
            {
                case "cleanroom":
                    return "Clean Room";
                case "wetchem":
                case "robin":
                    return "Wet Chemistry";
                case "all":
                    return null;
                default:
                    throw new Exception($"Unknown area: {alias}");
            }
        }

        public static DateTime? FixEventDate(DateTime? eventDate)
        {
            if (eventDate.HasValue && eventDate < DateTime.Parse("1990-01-01"))
            {
                int offset = 2013 - 1985;
                DateTime result = eventDate.Value.AddYears(offset);
                return result;
            }
            else
                return eventDate;
        }

        public static DateTime? AdjustEventDate(DateTime? eventDate)
        {
            if (eventDate.HasValue && eventDate >= DateTime.Parse("1990-01-01"))
            {
                int offset = 2013 - 1985;
                DateTime result = eventDate.Value.AddYears(-offset);
                return result;
            }
            else
                return eventDate;
        }

        public static string BytesToString(object value)
        {
            var bytes = (IEnumerable<byte>)value;
            StringBuilder hex = new StringBuilder(bytes.Count() * 2);
            foreach (byte b in bytes)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString().ToUpper();
        }

        public static IEnumerable<byte> StringToBytes(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static string GetEventDescription(string key, string value)
        {
            //some special case handling is needed, a number of cards expired however Prowatch still allowed them through
            if (ExpiredCardFix.ContainsKey(key) && value == "Expired Card Attempt")
                return ExpiredCardFix[key];

            //change Host to Local
            if (value == "Host Grant - IN")
                return "Local Grant - IN";

            if (value == "Host Grant - OUT")
                return "Local Grant - OUT";

            //not sure what to do so just return value back
            return value;
        }

        public static EventType GetEventType(string eventDescription)
        {
            if (EventTypeLookup.ContainsKey(eventDescription))
                return EventTypeLookup[eventDescription];
            else
                throw new ArgumentException(string.Format("Unknown value: {0}", eventDescription), "eventDescription");
        }

        public static bool IsPassbackRoom(Event e, IEnumerable<IRoom> rooms)
        {
            var room = rooms.FirstOrDefault(x => x.RoomID == e.RoomID);

            if (room == null)
                return false;
            else
                return room.PassbackRoom;
        }

        public static object DBNullIf(object value, bool test)
        {
            if (test)
                return DBNull.Value;
            else
                return value;
        }
    }
}
