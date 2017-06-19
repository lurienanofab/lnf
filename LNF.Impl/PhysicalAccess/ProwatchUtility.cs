using LNF.CommonTools;
using LNF.PhysicalAccess;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LNF.Impl.PhysicalAccess
{
    public static class ProwatchUtility
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

                int result = 0;

                if (!int.TryParse(value.ToString(), out result))
                    return null;

                return result;
            }

            return null;
        }

        public static Badge CreateBadge(DataRow row)
        {
            if (row == null) return null;
            Badge result = new Badge()
            {
                ID = row["ID"],
                ClientID = row.Field<int>("BADGE_CLIENTID"),
                UserName = row.Field<string>("BADGE_SSEL_UNAME"),
                LastName = row.Field<string>("LNAME"),
                FirstName = row.Field<string>("FNAME"),
                IssueDate = FixEventDate(row.Field<DateTime>("ISSUE_DATE")).Value,
                ExpireDate = FixEventDate(row.Field<DateTime>("EXPIRE_DATE")).Value,
                CurrentAccessTime = FixEventDate(row.Field<DateTime?>("EVENT_TIME")),
                CurrentCardNumber = GetCardNumber(row),
                CurrentEventDescription = row.Field<string>("EVENT_DESCRP"),
                CurrentAreaName = row.Field<string>("AREA_NAME"),
                AltDescription = row.Field<string>("ALT_DESCRP")
            };
            return result;
        }

        public static Card CreateCard(DataRow row)
        {
            if (row == null) return null;
            return new Card()
            {
                ID = row["ID"],
                ClientID = RepositoryUtility.ConvertTo(row["BADGE_CLIENTID"], 0),
                UserName = RepositoryUtility.ConvertTo(row["BADGE_SSEL_UNAME"], string.Empty),
                LastName = RepositoryUtility.ConvertTo(row["LNAME"], string.Empty),
                FirstName = RepositoryUtility.ConvertTo(row["FNAME"], string.Empty),
                Number = RepositoryUtility.ConvertTo(row["CARDNO"], 0),
                LastAccess = Utility.ConvertObjectToNullableDateTime(row["LAST_ACC"]),
                CardIssueDate = RepositoryUtility.ConvertTo(row["CARD_ISSUE_DATE"], DateTime.Now),
                CardExpireDate = RepositoryUtility.ConvertTo(row["CARD_EXPIRE_DATE"], DateTime.Now),
                BadgeIssueDate = RepositoryUtility.ConvertTo(row["BADGE_ISSUE_DATE"], DateTime.Now),
                BadgeExpireDate = RepositoryUtility.ConvertTo(row["BADGE_EXPIRE_DATE"], DateTime.Now),
                Status = GetCardStatus(RepositoryUtility.ConvertTo(row["STAT_COD"], string.Empty))
            };
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

        public static List<Event> CreateEvents(DataTable raw)
        {
            int[] clientIds = raw.AsEnumerable().Select(x => x.Field<int>("BADGE_CLIENTID")).Distinct().ToArray();

            var clients = DA.Current.Query<Client>().Where(x => clientIds.Contains(x.ClientID)).ToArray();
            var rooms = DA.Current.Query<Room>().Where(x => x.Active).ToArray();

            var result = new List<Event>();

            foreach (DataRow row in raw.Rows)
            {
                string eventId = BytesToString((byte[])row["RID"]);
                string deviceId = BytesToString((byte[])row["LOGDEVDTLID"]);
                string eventDesc = GetEventDescription(deviceId, row["EVNT_DESCRP"].ToString());
                string deviceDesc = row["LOGDEVADESCRP"].ToString();
                int cardnum = Convert.ToInt32(row["CARDNO"]); //this is a string padded to 32 length with trailing null chars

                var client = clients.FirstOrDefault(x => x.ClientID == row.Field<int>("BADGE_CLIENTID"));
                var room = rooms.FirstOrDefault(x => x.RoomName == deviceDesc && x.Active);

                if (room != null)
                {
                    result.Add(new Event()
                    {
                        ID = eventId,
                        DeviceID = deviceId,
                        Client = client,
                        Room = room,
                        EventType = GetEventType(eventDesc),
                        UserName = row["BADGE_SSEL_UNAME"].ToString(),
                        LastName = row["LNAME"].ToString(),
                        FirstName = row["FNAME"].ToString(),
                        EventDateTime = row.Field<DateTime>("EVNT_DAT"),
                        EventDescription = eventDesc,
                        DeviceDescription = deviceDesc,
                        CardNumber = cardnum,
                        CardStatus = GetCardStatus(row["STAT_COD"].ToString()),
                        CardIssueDate = row.Field<DateTime>("ISSUE_DAT"),
                        CardExpireDate = row.Field<DateTime>("EXPIRE_DAT")
                    });
                }
            }

            return result;
        }

        public static LNF.PhysicalAccess.Area CreateArea(DataRow row)
        {
            if (row == null) return null;

            return new LNF.PhysicalAccess.Area()
            {
                ID = ProwatchUtility.BytesToString((byte[])row["ID"]),
                Name = row["AREA_NAME"].ToString()
            };
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

        public static string BytesToString(IEnumerable<byte> bytes)
        {
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

        public static UnitOfWorkAdapter GetDBA()
        {
            var dba = new SQLDBAccess("cnProwatch");
            return dba;
        }
    }
}
