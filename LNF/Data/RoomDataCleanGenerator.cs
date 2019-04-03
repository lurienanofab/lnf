using LNF.Models.Data;
using LNF.Models.PhysicalAccess;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Data
{
    public class RoomDataCleanGenerator
    {
        private const double MAX_TIME = 8.0;
        private IEnumerable<ICost> _costs;

        protected IProvider Provider { get; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Client Client { get; set; }
        public Room Room { get; set; }
        
        public RoomDataCleanGenerator(IProvider provider, DateTime startDate, DateTime endDate, Client client = null, Room room = null)
        {
            Provider = provider;
            StartDate = startDate;
            EndDate = endDate;
            Client = client;
            Room = room;
        }

        public RoomDataClean[] Generate()
        {
            var events = ServiceProvider.Current.PhysicalAccess.GetEvents(StartDate, EndDate, Client.ClientID, Room.RoomID);

            var result = Filter(events);

            return result;
        }

        private IEnumerable<ICost> GetCurrentCosts()
        {
            if (_costs == null)
                _costs = Provider.CostManager.FindCosts(new[] { "RoomCost" }, EndDate).AsQueryable().CreateModels<ICost>();

            return _costs;
        }

        public DataTable EventsToDataTable(IEnumerable<Event> events)
        {
            var dt = new DataTable();

            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("DeviceID", typeof(string));
            dt.Columns.Add("ClientID", typeof(int));
            dt.Columns.Add("RoomID", typeof(int));
            dt.Columns.Add("EventType", typeof(string));
            dt.Columns.Add("UserName", typeof(string));
            dt.Columns.Add("LastName", typeof(string));
            dt.Columns.Add("FirstName", typeof(string));
            dt.Columns.Add("EventDateTime", typeof(DateTime));
            dt.Columns.Add("EventDescription", typeof(string));
            dt.Columns.Add("DeviceDescription", typeof(string));
            dt.Columns.Add("CardNumber", typeof(int));
            dt.Columns.Add("CardStatus", typeof(string));
            dt.Columns.Add("CardIssueDate", typeof(DateTime));
            dt.Columns.Add("CardExpireDate", typeof(DateTime));

            var rooms = DA.Current.Query<Room>().Where(x => x.Active).ToList();

            foreach (var e in events)
            {
                var dr = dt.NewRow();
                dr["ID"] = e.ID;
                dr["DeviceID"] = e.DeviceID;
                dr["ClientID"] = e.ClientID;
                dr["RoomID"] = rooms.First(x => x.RoomName == e.DeviceDescription).RoomID;
                dr["EventType"] = e.EventType.ToString();
                dr["UserName"] = e.UserName;
                dr["LastName"] = e.LastName;
                dr["FirstName"] = e.FirstName;
                dr["EventDateTime"] = e.EventDateTime;
                dr["EventDescription"] = e.EventDescription;
                dr["DeviceDescription"] = e.DeviceDescription;
                dr["CardNumber"] = e.CardNumber;
                dr["CardStatus"] = e.CardStatus;
                dr["CardIssueDate"] = e.CardIssueDate;
                dr["CardExpireDate"] = e.CardExpireDate;
                dt.Rows.Add(dr);
            }

            return dt;
        }

        private RoomDataClean[] Filter(IEnumerable<Event> events)
        {
            List<RoomDataClean> result = new List<RoomDataClean>();

            if (events.Count() == 0)
                return result.ToArray();

            int[] clientIds = events.Select(x => x.ClientID).Distinct().ToArray();
            var clients = DA.Current.Query<Client>().Where(x => clientIds.Contains(x.ClientID));

            string[] roomNames = events.Where(x => x.DeviceDescription != null).Select(x => x.DeviceDescription).Distinct().ToArray();
            var rooms = DA.Current.Query<Room>().Where(x => roomNames.Contains(x.RoomName));

            //loop through clients, extract their lab usage data, subset it and then clean
            Event[] fdr;
            Event[] clean;
            DateTime entry, exit;

            foreach (var client in clients)
            {
                foreach (var room in rooms)
                {
                    if (IsChargeable(room)) //only store chargeable rooms
                    {
                        fdr = events.Where(x => x.ClientID == client.ClientID && x.DeviceDescription == room.RoomName).OrderBy(x => x.EventDateTime).ToArray();
                        if (fdr.Length > 0)
                        {
                            clean = DataCleaner(fdr, client, room).ToArray();

                            if (clean.Length > 0)
                            {
                                //loop through the data and put into the proper format
                                int step = room.PassbackRoom ? 2 : 1;
                                Event clean0, clean1;

                                for (int i = 0; i < clean.Length; i += step)
                                {
                                    //if (dtClean.Rows[i].RowState != DataRowState.Deleted)
                                    //{
                                    if (room.PassbackRoom)
                                    {
                                        clean0 = clean[i];
                                        clean1 = clean[i + 1];
                                        entry = clean0.EventDateTime;
                                        exit = clean1.EventDateTime;
                                        if (exit != EndDate) //will filter events that get cut at the eDate boundary
                                        {
                                            result.Add(new RoomDataClean()
                                            {
                                                ClientID = client.ClientID,
                                                RoomID = room.RoomID,
                                                EntryDT = entry,
                                                ExitDT = exit,
                                                Duration = exit.Subtract(entry).TotalHours
                                            });
                                        }
                                    }
                                    else
                                    {
                                        //Wen: non passback room handling
                                        clean0 = clean[i];
                                        entry = clean0.EventDateTime;
                                        result.Add(new RoomDataClean()
                                        {
                                            ClientID = client.ClientID,
                                            RoomID = room.RoomID,
                                            EntryDT = entry,
                                            ExitDT = entry, //It's better to have something in ExitDT, because if it's null it's difficult to do query
                                            Duration = 0 //Wen: since non antipassback room cannot have the "out" event, its duration is meaningless
                                        });
                                    }
                                    //}
                                    //else
                                    //    i -= 1;
                                }
                            }
                        }
                    }
                }
            }

            //now, remove all rows that occurred in the past day - can happen if first entry is PB error
            //also remove any entries that are in the future
            result.RemoveAll(x => (x.EntryDT < StartDate && x.ExitDT < StartDate) || x.ExitDT > DateTime.Now);

            //remove all rows that begin or end on either boundary and whose duration is maxtime
            result.RemoveAll(x => (x.EntryDT == StartDate || x.ExitDT == EndDate) && x.Duration == MAX_TIME);

            return result.ToArray();
        }

        private bool IsChargeable(Room room)
        {
            return GetCurrentCosts().Where(x => x.RecordID == room.RoomID && x.AcctPer != "None").Any();
        }

        //2007-11-09 Wen: Clean the anitpassback error records
        private Event[] DataCleaner(IEnumerable<Event> events, Client client, Room room)
        {
            if (room.PassbackRoom)
                return CleanPassbackRoomData(events, client, room); //passing in client and room for convenience
            else
                return CleanNonPassbackRoomData(events);
        }

        private Event[] CleanPassbackRoomData(IEnumerable<Event> events, Client client, Room room)
        {
            return events.ToArray();

            //IList<Event> result = events.ToList();

            //int i = 0;
            //Event e;
            //DateTime evStart = StartDate; //if first record is out, this will start at beginning of period
            //DateTime dateTime1, dateTime2;
            //DateTime dtAP1, dtAPn;

            ////the function newCleanRow works because
            ////this function is always run one user at a time
            ////room is passed in for this purpose

            ////changed from < to <= to catch the case of the last row being 'wrong'
            //while (i <= result.Count - 1) //this allows checking the next row easily
            //{
            //    switch (result[i].EventDescription)
            //    {
            //        case "Local Grant - IN":
            //            switch (result[i + 1].EventDescription)
            //            {
            //                case "Local Grant - IN":
            //                    //2009-11-23 The clean room double door system usually cause problems for users on multiple badge-in or badge-out instances.
            //                    //  The system always consider it's IN/OUT while the user is still not in/out of clean room. To prevent from overcharging
            //                    //  the users, we set 5 minutes minimum policy - if there are two concurring INs and the time between them is less than 5 minutes,
            //                    //  we delete that IN entry. Same thing to OUT entry

            //                    DateTime dateTimeTest = result[i].EventDateTime;
            //                    dateTime2 = result[i + 1].EventDateTime.AddSeconds(-1);

            //                    if ((dateTime2 - dateTimeTest).Seconds > 300)
            //                    {
            //                        dateTime1 = result[i].EventDateTime.AddHours(MAX_TIME);
            //                        dr = newCleanRow(dt, clientId, roomId);
            //                        dr["evtType"] = "Local Grant - OUT";
            //                        if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
            //                            dr["evtDate"] = dateTime2;
            //                        else
            //                            dr["evtDate"] = dateTime1;
            //                        dt.Rows.InsertAt(dr, i + 1); //i not incremented, will become normal case
            //                    }
            //                    else
            //                    {
            //                        //less than 5 minutes, so we delete this row
            //                        dt.Rows[i].AcceptChanges();
            //                        dt.Rows[i].Delete();
            //                        i += 1;
            //                    }
            //                    break;
            //                case "Local Grant - OUT":
            //                    dateTime1 = Convert.ToDateTime(dt.Rows[i]["evtDate"]);
            //                    dateTime2 = Convert.ToDateTime(dt.Rows[i + 1]["evtDate"]);
            //                    if (dateTime2.Subtract(dateTime1).TotalHours > MaxTime)
            //                    {
            //                        dr = newCleanRow(dt, clientId, roomId);
            //                        dr["evtType"] = "Local Grant - OUT";
            //                        dr["evtDate"] = dateTime1.AddHours(MaxTime);
            //                        dt.Rows.InsertAt(dr, i + 1); //i not incremented, will become normal case

            //                        if (dateTime2.Subtract(dateTime1).TotalHours > 2 * MaxTime)
            //                            dateTime2 = dateTime2.AddHours(-MaxTime);
            //                        else
            //                            dateTime2 = dateTime1.AddHours(MaxTime).AddSeconds(1);
            //                        dr = newCleanRow(dt, clientId, roomId);
            //                        dr["evtType"] = "Local Grant - IN";
            //                        dr["evtDate"] = dateTime2;
            //                        dt.Rows.InsertAt(dr, i + 2); //i not incremented, will become normal case
            //                    }
            //                    else
            //                        i = i + 2;
            //                    break;
            //                case "Antipassback error - IN":
            //                    dtAP1 = Convert.ToDateTime(dt.Rows[i + 1]["evtDate"]);
            //                    dtAPn = dtAP1;
            //                    while (dt.Rows[i + 1]["evtType"].ToString() == "Antipassback error - IN" && i + 1 < dt.Rows.Count - 1)
            //                    {
            //                        dtAPn = Convert.ToDateTime(dt.Rows[i + 1]["evtDate"]);
            //                        dt.Rows.Remove(dt.Rows[i + 1]);
            //                    }
            //                    if (dt.Rows[i + 1]["evtType"].ToString() == "Local Grant - IN")
            //                    {
            //                        dr = newCleanRow(dt, clientId, roomId);
            //                        dr["evtType"] = "Local Grant - OUT";
            //                        dateTime1 = dtAP1.AddSeconds(-1);
            //                        dateTime2 = Convert.ToDateTime(dt.Rows[i]["evtDate"]).AddHours(MaxTime);
            //                        if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
            //                            dr["evtDate"] = dateTime2;
            //                        else
            //                            dr["evtDate"] = dateTime1;
            //                        dt.Rows.InsertAt(dr, i + 1); //i not incremented, will become normal case
            //                    }
            //                    else
            //                    {
            //                        dateTime1 = Convert.ToDateTime(dt.Rows[i]["evtDate"]);
            //                        dateTime2 = dtAPn;
            //                        TimeSpan timeDiff = dateTime2.Subtract(dateTime1);
            //                        if (timeDiff.TotalSeconds > 300.0)
            //                        {
            //                            dr = newCleanRow(dt, clientId, roomId);
            //                            dr["evtType"] = "Local Grant - OUT";
            //                            dateTime1 = Convert.ToDateTime(dt.Rows[i]["evtDate"]).AddHours(MaxTime);
            //                            dateTime2 = dtAP1.AddSeconds(-1);
            //                            if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
            //                                dr["evtDate"] = dateTime2;
            //                            else
            //                                dr["evtDate"] = dateTime1;
            //                            dt.Rows.InsertAt(dr, i + 1); //i not incremented, will become normal case

            //                            dr = newCleanRow(dt, clientId, roomId);
            //                            dr["evtType"] = "Local Grant - IN";
            //                            dateTime1 = dtAPn.AddSeconds(1);
            //                            dateTime2 = Convert.ToDateTime(dt.Rows[i + 1]["evtDate"]).AddHours(-MaxTime);
            //                            if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
            //                                dr["evtDate"] = dateTime1;
            //                            else
            //                                dr["evtDate"] = dateTime2;
            //                            dt.Rows.InsertAt(dr, i + 2); //i not incremented, will become normal case
            //                        }
            //                    }
            //                    break;
            //                case "Antipassback error - OUT":
            //                    dtAP1 = Convert.ToDateTime(dt.Rows[i + 1]["evtDate"]);
            //                    dtAPn = dtAP1;
            //                    while (dt.Rows[i + 1]["evtType"].ToString() == "Antipassback error - OUT" && i + 1 < dt.Rows.Count - 1)
            //                    {
            //                        dtAPn = Convert.ToDateTime(dt.Rows[i + 1]["evtDate"]);
            //                        dt.Rows.Remove(dt.Rows[i + 1]);
            //                    }
            //                    if (dt.Rows[i + 1]["evtType"].ToString() == "Local Grant - IN")
            //                    {
            //                        dr = newCleanRow(dt, clientId, roomId);
            //                        dr["evtType"] = "Local Grant - OUT";

            //                        dateTime1 = dtAPn.AddSeconds(-1);
            //                        dateTime2 = Convert.ToDateTime(dt.Rows[i]["evtDate"]).AddHours(MaxTime);
            //                        if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
            //                            dr["evtDate"] = dateTime2;
            //                        else
            //                            dr["evtDate"] = dateTime1;
            //                        dt.Rows.InsertAt(dr, i + 1); //i not incremented, will become normal case
            //                    }
            //                    break;
            //                default:
            //                    dt.Rows.Remove(dt.Rows[i + 1]); //to pick up cases like "Local Grant"
            //                    break;
            //            }
            //            break;
            //        case "Local Grant - OUT":
            //            dateTime1 = Convert.ToDateTime(dt.Rows[i - 1]["evtDate"]).AddSeconds(1);
            //            dateTime2 = Convert.ToDateTime(dt.Rows[i]["evtDate"]).AddHours(-MaxTime);
            //            dr = newCleanRow(dt, clientId, roomId);
            //            dr["evtType"] = "Local Grant - IN";
            //            if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
            //                dr["evtDate"] = dateTime1;
            //            else
            //                dr["evtDate"] = dateTime2;
            //            dt.Rows.InsertAt(dr, i); //i not incremented, will become normal case
            //            break;
            //        case "Antipassback error - IN":
            //            dtAP1 = Convert.ToDateTime(dt.Rows[i]["evtDate"]);
            //            while (dt.Rows[i]["evtType"].ToString() == "Antipassback error - IN" && i < dt.Rows.Count - 1)
            //            {
            //                dt.Rows.Remove(dt.Rows[i]);
            //            }
            //            if (dt.Rows[i]["evtType"].ToString() == "Local Grant - OUT") //what about ap-out?
            //            {
            //                dr = newCleanRow(dt, clientId, roomId);
            //                dr["evtType"] = "Local Grant - IN";
            //                dateTime1 = dtAP1;
            //                dateTime2 = Convert.ToDateTime(dt.Rows[i]["evtDate"]).AddHours(-MaxTime);
            //                if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
            //                    dr["evtDate"] = dateTime1;
            //                else
            //                    dr["evtDate"] = dateTime2;
            //                dt.Rows.InsertAt(dr, i); //i not incremented, will become normal case
            //            }
            //            break;
            //        case "Antipassback error - OUT":
            //            dtAP1 = Convert.ToDateTime(dt.Rows[i - 1]["evtDate"]);
            //            while (dt.Rows[i]["evtType"].ToString() == "Antipassback error - OUT" && i < dt.Rows.Count - 1)
            //            {
            //                dt.Rows.Remove(dt.Rows[i]);
            //            }
            //            if (dt.Rows[i]["evtType"].ToString() == "Local Grant - IN") //what about ap-out?
            //            {
            //                dtAPn = Convert.ToDateTime(dt.Rows[i]["evtDate"]);
            //                TimeSpan timeDiff = dtAPn.Subtract(dtAP1);
            //                if (timeDiff.TotalSeconds > 300.0)
            //                {
            //                    dr = newCleanRow(dt, clientId, roomId);
            //                    dr["evtType"] = "Local Grant - IN";
            //                    dateTime1 = Convert.ToDateTime(dt.Rows[i - 1]["evtDate"]).AddSeconds(1); //when I left last
            //                    dateTime2 = dtAPn.AddHours(-MaxTime);
            //                    if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
            //                        dr["evtDate"] = dateTime1;
            //                    else
            //                        dr["evtDate"] = dateTime2;
            //                    dt.Rows.InsertAt(dr, i); //i not incremented, will become normal case

            //                    dr = newCleanRow(dt, clientId, roomId);
            //                    dr["evtType"] = "Local Grant - OUT";
            //                    dr["evtDate"] = dtAPn;
            //                    dt.Rows.InsertAt(dr, i + 1); //i not incremented, will become normal case
            //                }
            //            }
            //            else
            //            {
            //                //don't know what this means, but occured due to algorithm with Beach vendor in Jan 04
            //                dr = newCleanRow(dt, clientId, roomId);
            //                dr["evtType"] = "Local Grant - IN";
            //                dateTime1 = dtAP1;
            //                dateTime2 = Convert.ToDateTime(dt.Rows[i]["evtDate"]).AddHours(-MaxTime);
            //                if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
            //                    dr["evtDate"] = dateTime1;
            //                else
            //                    dr["evtDate"] = dateTime2;
            //                dt.Rows.InsertAt(dr, i); //i not incremented, will become normal case
            //            }
            //            break;
            //        default:
            //            dt.Rows.Remove(dt.Rows[i]); //to pick up cases like "Local Grant"
            //            break;
            //    }
            //}

            ////if there is only a single event and it spans the entire period, treat it as bogus
            //double hoursInPeriod = eDate.Subtract(sDate).TotalHours;
            //if (dt.Rows.Count == 2 && (Convert.ToDateTime(dt.Rows[1]["evtDate"]).Subtract(Convert.ToDateTime(dt.Rows[0]["evtDate"]))).TotalHours == hoursInPeriod)
            //    dt.Rows.Clear();

            //return dt;
        }

        private Event NewEvent(Client client, Room room)
        {
            return new Event()
            {
                ClientID = client.ClientID,
                DeviceDescription = room.RoomName
            };
        }

        private Event[] CleanNonPassbackRoomData(IEnumerable<Event> events)
        {
            return events.Where(x => !x.EventDescription.Contains("Antipassback")).ToArray();
        }
    }
}
