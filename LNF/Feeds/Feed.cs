using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Data;
using System.Text;

namespace LNF.Feeds
{
    public class Feed
    {
        private FeedFormats _Format;
        private DataTable _Data;
        private string _ContentType;
        private string _Charset;
        private string _FileExtension;

        internal Feed(FeedFormats format, DataTable dt)
        {
            _Format = format;
            _Data = dt;

            switch (_Format)
            {
                case FeedFormats.Calendar:
                    _ContentType = "text/calendar";
                    _Charset = string.Empty;
                    _FileExtension = ".ics";
                    break;
                default:
                    throw new NotImplementedException("Feed format has not been implemented.");
            }
        }

        public FeedFormats Format { get { return _Format; } }
        public string ContentType { get { return _ContentType; } }
        public string Charset { get { return _Charset; } }
        public string FileExtension { get { return _FileExtension; } }

        public string GetFileNameWithExtension(string fileName)
        {
            return fileName + FileExtension;
        }

        public string Render(string feedName)
        {
            StringBuilder sb = new StringBuilder();

            switch (_Format)
            {
                case FeedFormats.Calendar:

                    string serverIp = Providers.Context.Current.ServerVariables["LOCAL_ADDR"];
                    DateTime utc_build_time = DateTime.UtcNow;

                    sb.AppendLine("BEGIN:VCALENDAR");
                    sb.AppendLine("METHOD:PUBLISH");
                    sb.AppendLine("PRODID:-//" + serverIp + "//NONSGML LNF-ICAL 1.0//");
                    sb.AppendLine("VERSION:2.0");
                    sb.AppendLine("X-WR-CALNAME:" + feedName);
                    sb.AppendLine("X-WR-CALDESC:LNF OnLine Services Data Feed");
                    sb.AppendLine("X-WR-TIMEZONE:US-Eastern");
                    int i = 0;
                    foreach (DataRow dr in _Data.Rows)
                    {
                        dr["UID"] = RepositoryUtility.ConvertTo(dr["UID"], "feed_item_" + i.ToString()) + "@" + serverIp;
                        if (dr["DTSTAMP"] == DBNull.Value)
                            dr["DTSTAMP"] = utc_build_time.ToString("yyyyMMdd'T'HHmmss'Z'");
                        if (dr["LOCATION"] == DBNull.Value)
                            dr["LOCATION"] = "Lurie Nanofabrication Facility";

                        /*
                        BEGIN:VEVENT
                        DTSTART;TZID=America/New_York:20120130T083000s
                        DTEND;TZID=America/New_York:20120130T093000
                        RRULE:FREQ=WEEKLY;BYDAY=MO,TH
                        DTSTAMP:20120321T175213Z
                        UID:q995t2lechpmlojm87ag3pbf58@google.com
                        CREATED:20120320T042349Z
                        DESCRIPTION:
                        LAST-MODIFIED:20120320T042616Z
                        LOCATION:LNF Cleanroom
                        SEQUENCE:1
                        STATUS:CONFIRMED
                        SUMMARY:Lab Clean
                        TRANSP:OPAQUE
                        END:VEVENT
                        */

                        sb.AppendLine("BEGIN:VEVENT");
                        AddCalendarProperty(sb, dr, "DTSTART");
                        AddCalendarProperty(sb, dr, "DTEND");
                        AddCalendarProperty(sb, dr, "RRULE");
                        AddCalendarProperty(sb, dr, "DTSTAMP");
                        AddCalendarProperty(sb, dr, "UID");
                        AddCalendarProperty(sb, dr, "CREATED");
                        AddCalendarProperty(sb, dr, "DESCRIPTION");
                        AddCalendarProperty(sb, dr, "LAST-MODIFIED");
                        AddCalendarProperty(sb, dr, "LOCATION");
                        AddCalendarProperty(sb, dr, "SEQUENCE");
                        AddCalendarProperty(sb, dr, "STATUS");
                        AddCalendarProperty(sb, dr, "SUMMARY");
                        AddCalendarProperty(sb, dr, "TRANSP");
                        sb.AppendLine("END:VEVENT");

                        i++;
                    }
                    sb.Append("END:VCALENDAR");

                    break;
            }

            return sb.ToString();
        }

        private void AddCalendarProperty(StringBuilder sb, DataRow dr, string key)
        {
            if (dr.Table.Columns.Contains(key) && dr[key] != DBNull.Value && !string.IsNullOrEmpty(dr[key].ToString()))
            {
                string value = dr[key].ToString();
                sb.AppendLine(key + ((value.StartsWith(";") || value.StartsWith(":")) ? string.Empty : ":") + dr[key].ToString());
            }
        }

        public void WriteLogEntry(IContext context)
        {
            FeedsLog log = new FeedsLog();
            log.EntryDateTime = DateTime.Now;
            log.RequestIP = GetIP(context);
            log.RequestURL = context.GetRequestUrl().ToString();
            log.RequestUserAgent = context.GetRequestUserAgent() ?? "unknown";
            DA.Current.Insert(log);
        }

        private string GetIP(IContext context)
        {
            string ip = context.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrEmpty(ip))
            {
                string[] addresses = ip.Split(',');
                if (addresses.Length != 0)
                    return addresses[0];
            }

            return context.ServerVariables["REMOTE_ADDR"];
        }
    }
}
