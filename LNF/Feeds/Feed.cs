using LNF.Data;
using System;
using System.Data;
using System.Text;

namespace LNF.Feeds
{
    public class Feed
    {
        private DataTable _Data;

        internal Feed(FeedFormats format, DataTable dt)
        {
            Format = format;
            _Data = dt;

            switch (Format)
            {
                case FeedFormats.Calendar:
                    ContentType = "text/calendar";
                    Charset = string.Empty;
                    FileExtension = ".ics";
                    break;
                default:
                    throw new NotImplementedException("Feed format has not been implemented.");
            }
        }

        public FeedFormats Format { get; }
        public string ContentType { get; }
        public string Charset { get; }
        public string FileExtension { get; }

        public string GetFileNameWithExtension(string fileName)
        {
            return fileName + FileExtension;
        }

        public string Render(string feedName, string serverIp)
        {
            StringBuilder sb = new StringBuilder();

            switch (Format)
            {
                case FeedFormats.Calendar:
                    DateTime utc_build_time = DateTime.UtcNow;

                    sb.AppendLine("BEGIN:VCALENDAR");
                    sb.AppendLine("METHOD:PUBLISH");
                    sb.AppendLine($"PRODID:-//{serverIp}//NONSGML {GlobalSettings.Current.CompanyName}-ICAL 1.0//");
                    sb.AppendLine("VERSION:2.0");
                    sb.AppendLine("X-WR-CALNAME:" + feedName);
                    sb.AppendLine($"X-WR-CALDESC:{GlobalSettings.Current.CompanyName} Online Services Data Feed");
                    sb.AppendLine("X-WR-TIMEZONE:US-Eastern");
                    int i = 0;
                    foreach (DataRow dr in _Data.Rows)
                    {
                        dr["UID"] = CommonTools.Utility.ConvertTo(dr["UID"], "feed_item_" + i.ToString()) + "@" + serverIp;
                        if (dr["DTSTAMP"] == DBNull.Value)
                            dr["DTSTAMP"] = utc_build_time.ToString("yyyyMMdd'T'HHmmss'Z'");
                        if (dr["LOCATION"] == DBNull.Value)
                            dr["LOCATION"] = "Lurie Nanofabrication Facility";

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

        public IFeedsLog WriteLogEntry(string requestIp, string requestUrl, string userAgent)
        {
            IFeedsLog log = ServiceProvider.Current.Data.Feed.AddFeedsLogEntry(requestIp, requestUrl, userAgent);
            return log;
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
