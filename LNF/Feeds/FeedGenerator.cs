using LNF.Data;
using LNF.Help;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Feeds
{
    public enum FeedFormats
    {
        Calendar = 1
    }

    public static class FeedGenerator
    {
        public static class Help
        {
            public static class StaffHours
            {
                public static string GetUrl(FeedFormats format, string userName, Uri requestUri)
                {
                    string result = string.Empty;
                    result = requestUri.GetLeftPart(UriPartial.Authority);
                    result += "/feeds/help/staffhours/" + FeedFormatToString(format) + "/" + userName;
                    return result;
                }

                public static DateTime[] WeekArray(DateTime d)
                {
                    DateTime[] result = new DateTime[7];
                    for (int i = 0; i < 7; i++)
                    {
                        result[i] = d.Date.AddDays(i - (int)d.DayOfWeek);
                    }
                    return result;
                }

                public static string DayCode(DayOfWeek dow)
                {
                    switch (dow)
                    {
                        case DayOfWeek.Sunday:
                            return "SU";
                        case DayOfWeek.Monday:
                            return "MO";
                        case DayOfWeek.Tuesday:
                            return "TU";
                        case DayOfWeek.Wednesday:
                            return "WE";
                        case DayOfWeek.Thursday:
                            return "TH";
                        case DayOfWeek.Friday:
                            return "FR";
                        case DayOfWeek.Saturday:
                            return "SA";
                        default:
                            throw new ArgumentException("dow");
                    }
                }

                public static string TimeRangeToString(DateTime sd, DateTime ed)
                {
                    string result = string.Empty;
                    result += (sd.Hour <= 12) ? sd.Hour.ToString() : (sd.Hour - 12).ToString();
                    if (sd.Minute > 0)
                        result += ":" + (sd.Minute + 100).ToString().Substring(1);
                    result += (sd.Hour < 12) ? "am" : "pm";
                    result += " - ";
                    result += (ed.Hour <= 12) ? ed.Hour.ToString() : (ed.Hour - 12).ToString();
                    if (ed.Minute > 0)
                        result += ":" + (ed.Minute + 100).ToString().Substring(1);
                    result += (ed.Hour < 12) ? "am" : "pm";
                    return result;
                }

                public static Feed CreateFeed(FeedFormats format, string userName)
                {
                    switch (format)
                    {
                        case FeedFormats.Calendar:
                            DataTable dt = FeedGenerator.InitCalendarTable();
                            Dictionary<string, object> search_params = new Dictionary<string, object>();
                            IStaffDirectory sd = null;
                            if (!string.IsNullOrEmpty(userName))
                                sd = ServiceProvider.Current.Data.Client.GetStaffDirectory(userName);
                            if (sd != null)
                            {
                                DateTime[] week = WeekArray(sd.LastUpdate);
                                StaffTimeInfoCollection staffTime = new StaffTimeInfoCollection(sd.HoursXML);
                                IClient c = ServiceProvider.Current.Data.Client.GetClient(sd.ClientID);
                                _ = new StaffDirectoryEntry()
                                {
                                    StaffDirectoryID = sd.StaffDirectoryID,
                                    ClientID = sd.ClientID,
                                    UserName = c.UserName,
                                    Privs = c.Privs,
                                    LName = c.LName,
                                    MName = c.MName,
                                    FName = c.FName,
                                    Hours = staffTime.ToString(),
                                    Email = c.Email,
                                    Phone = c.Phone,
                                    Office = sd.Office,
                                    Deleted = sd.Deleted,
                                    ReadOnly = true
                                };

                                foreach (KeyValuePair<DayOfWeek, StaffTimeInfo> kvp in staffTime)
                                {
                                    if (kvp.Value.Checked)
                                    {
                                        DataRow dr = dt.NewRow();
                                        DateTime dtstart = week[(int)kvp.Key].Add(kvp.Value.AM.Start.Value.Value);
                                        DateTime dtend = week[(int)kvp.Key].Add(kvp.Value.PM.End.Value.Value);
                                        DateTime lunchStart = week[(int)kvp.Key].Add(kvp.Value.AM.End.Value.Value);
                                        DateTime lunchEnd = week[(int)kvp.Key].Add(kvp.Value.PM.Start.Value.Value);
                                        dr["DTSTART"] = ";TZID=America/New_York:" + dtstart.ToString("yyyyMMddTHHmmss");
                                        dr["DTEND"] = ";TZID=America/New_York:" + dtend.ToString("yyyyMMddTHHmmss");
                                        dr["RRULE"] = ":FREQ=WEEKLY;BYDAY=" + DayCode(kvp.Key);
                                        dr["DTSTAMP"] = ":" + DateTime.Now.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
                                        dr["UID"] = ":staffdirectory_" + sd.StaffDirectoryID.ToString();
                                        dr["CREATED"] = ":" + sd.LastUpdate.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
                                        dr["LAST-MODIFIED"] = ":" + sd.LastUpdate.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
                                        dr["STATUS"] = ":CONFIRMED";
                                        dr["SUMMARY"] = ":" + sd.LName + " " + TimeRangeToString(dtstart, dtend);
                                        dr["DESCRIPTION"] = ":Lunch: " + TimeRangeToString(lunchStart, lunchEnd);
                                        //BEGIN:VEVENT
                                        //DTSTART;TZID=America/New_York:20120914T080000
                                        //DTEND;TZID=America/New_York:20120914T170000
                                        //RRULE:FREQ=WEEKLY;BYDAY=FR
                                        //DTSTAMP:20120911T212356Z
                                        //UID:j550qf7o437fjj7l1dqeludg6o@google.com
                                        //CREATED:20120911T212251Z
                                        //LAST-MODIFIED:20120911T212251Z
                                        //STATUS:CONFIRMED
                                        //SUMMARY:Getty\, James 8am - 5pm
                                        //DESCRIPTION:Lunch: 12pm - 12:30pm
                                        //SEQUENCE:0
                                        //END:VEVENT
                                        //dr["DTSTART"] = week[(int)kvp.Key].Add(kvp.Value.AM.Start.Value.Value).ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'");
                                        //dr["DTEND"] = week[(int)kvp.Key].Add(kvp.Value.PM.End.Value.Value).ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'");
                                        //dr["UID"] = sd.StaffDirectoryID;
                                        //dr["CREATED"] = DateTime.Now.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'");
                                        //dr["LAST-MODIFIED"] = DateTime.Now.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'");
                                        //dr["STATUS"] = "CONFIRMED";
                                        //dr["SUMMARY"] = string.Format("{0} | {1} ({2})", item.ResourceName, item.DisplayName, item.Email);
                                        //dr["DESCRIPTION"] = string.Format(
                                        //    "Activity: {0}\\nStatus: {1}\\nScheduled Start: {2}\\nScheduled End: {3}\\nActual Start: {4}\\nActual End: {5}",
                                        //    item.ActivityName, GetReservationStatus(item), GetDateTime(item.BeginDateTime), GetDateTime(item.EndDateTime), GetDateTime(item.ActualBeginDateTime), GetDateTime(item.ActualEndDateTime)
                                        //);
                                        dt.Rows.Add(dr);
                                    }
                                }
                            }
                            return new Feed(format, dt);
                        default:
                            throw new NotImplementedException("Feed format has not been implemented.");
                    }
                }
            }
        }

        public static class Scheduler
        {
            public static class Reservations
            {
                public static string GetUrl(FeedFormats format, string userName, string resourceId, string fileName, Uri requestUri)
                {
                    string result = string.Empty;
                    result = requestUri.GetLeftPart(UriPartial.Authority);
                    result += "/feeds/scheduler/reservations/" + FeedFormatToString(format) + "/" + userName + "/" + resourceId + "/" + fileName;
                    return result;
                }

                public static IEnumerable<IReservationFeed> AllReservationInDateRange(DateTime sd, DateTime ed, int resourceId = 0)
                {
                    return ServiceProvider.Current.Data.Feed.GetReservationFeeds(sd, ed, resourceId);
                }

                public static IEnumerable<IReservationFeed> AllReservationInDateRange(string username, DateTime sd, DateTime ed, int resourceId = 0)
                {
                    return ServiceProvider.Current.Data.Feed.GetReservationFeeds(username, sd, ed, resourceId);
                }

                public static int GetResourceID(string resourceId)
                {
                    if (resourceId == "all")
                        return 0;

                    if (int.TryParse(resourceId, out int result))
                        return result;
                    else
                        return 0;
                }

                public static Feed CreateFeed(FeedFormats format, string username, string resourceId)
                {
                    switch (format)
                    {
                        case FeedFormats.Calendar:
                            DateTime d = DateTime.Now.Date;
                            DateTime sd = d.AddDays(-1);
                            DateTime ed = d.AddMonths(4);
                            DataTable dt = InitCalendarTable();
                            int rid = GetResourceID(resourceId);

                            IEnumerable<IReservationFeed> reservations;

                            // if there is username we must also look for invitees
                            if (!string.IsNullOrEmpty(username) && username != "all")
                                reservations = AllReservationInDateRange(username, sd, ed, rid);
                            else
                                reservations = AllReservationInDateRange(sd, ed, rid);

                            int facilityDowntimeActivityID = 23;
                            var items = reservations.Where(x => x.ActivityID != facilityDowntimeActivityID).ToList();

                            foreach (var item in items)
                            {
                                DataRow dr = dt.NewRow();
                                dr["DTSTART"] = item.BeginDateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'");
                                dr["DTEND"] = item.EndDateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'");
                                dr["UID"] = item.ReservationID.ToString();
                                dr["CREATED"] = item.CreatedOn.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'");
                                dr["LAST-MODIFIED"] = item.LastModifiedOn.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'");
                                dr["STATUS"] = (item.IsActive) ? "CONFIRMED" : "CANCELLED";
                                if (!item.IsActive) dr["METHOD"] = "CANCEL";
                                dr["SUMMARY"] = string.Format("{0} [{1}] | {2} ({3})", item.ResourceName, item.ResourceID, Clients.GetDisplayName(item.LName, item.FName), item.Email);
                                dr["DESCRIPTION"] = string.Format(
                                    "Activity: {0}\\nStatus: {1}\\nScheduled Start: {2}\\nScheduled End: {3}\\nActual Start: {4}\\nActual End: {5}",
                                    item.ActivityName, GetReservationStatus(item), GetDateTime(item.BeginDateTime), GetDateTime(item.EndDateTime), GetDateTime(item.ActualBeginDateTime), GetDateTime(item.ActualEndDateTime)
                                );
                                dt.Rows.Add(dr);
                            }
                            return new Feed(format, dt);
                        default:
                            throw new NotImplementedException("Feed format has not been implemented.");
                    }
                }
                private static string GetReservationStatus(IReservationFeed item)
                {
                    string result = "unknown";
                    if (item.IsActive && item.IsStarted && item.ActualEndDateTime == DateTime.MinValue)
                        result = "Running";
                    if (item.IsActive && item.IsStarted && item.ActualEndDateTime != DateTime.MinValue)
                        result = "Completed";
                    if (item.IsActive && !item.IsStarted)
                        result = "Waiting to Start";
                    if (!item.IsActive)
                        result = "Canceled";
                    return result;
                }

                private static string GetDateTime(DateTime? d)
                {
                    if (d == null)
                        return string.Empty;
                    else
                        return d.Value.ToString("dddd, MMMM dd yyyy h:mm tt");
                }
            }
        }

        public static string FeedFormatToString(FeedFormats format)
        {
            return Enum.GetName(typeof(FeedFormats), format).ToLower();
        }

        public static DataTable InitCalendarTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("DTSTART", typeof(string));
            dt.Columns.Add("DTEND", typeof(string));
            dt.Columns.Add("RRULE", typeof(string));
            dt.Columns.Add("DTSTAMP", typeof(string));
            dt.Columns.Add("UID", typeof(string));
            dt.Columns.Add("CREATED", typeof(string));
            dt.Columns.Add("DESCRIPTION", typeof(string));
            dt.Columns.Add("LAST-MODIFIED", typeof(string));
            dt.Columns.Add("LOCATION", typeof(string));
            dt.Columns.Add("SEQUENCE", typeof(string));
            dt.Columns.Add("STATUS", typeof(string));
            dt.Columns.Add("METHOD", typeof(string));
            dt.Columns.Add("SUMMARY", typeof(string));
            dt.Columns.Add("TRANSP", typeof(string));
            return dt;
        }
    }
}
