using LNF.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;

namespace LNF.Data
{
    public class DataFeedUtility
    {
        public IProvider Provider { get; }

        public DataFeedUtility(IProvider provider)
        {
            Provider = provider;
        }

        public static DataCommandBase ReadOnlyCommand(CommandType type = CommandType.StoredProcedure) => ReadOnlyDataCommand.Create(type);

        public static DataTable GetDataTable(string alias, object parameters)
        {
            string query = CommonTools.Utility.GetQueryString(parameters);
            string url = $"http://ssel-sched.eecs.umich.edu/data/feed/{alias}/xml/{query}";
            DataSet ds = new DataSet();
            ds.ReadXml(url);
            DataTable dt = ds.Tables[0];
            return dt;
        }

        public static string GetCSV(string alias, object parameters, string host = "ssel-sched.eecs.umich.edu")
        {
            string result = string.Empty;
            string query = CommonTools.Utility.GetQueryString(parameters);
            string url = $"http://{host}/data/feed/{alias}/csv/{query}";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
            {
                result = reader.ReadToEnd();
                reader.Close();
                resp.Close();
            }
            return result;
        }

        public static IEnumerable<T> GetData<T>(string alias, object parameters)
        {
            List<T> result = new List<T>();
            DataTable dt = GetDataTable(alias, parameters);
            foreach (DataRow dr in dt.Rows)
            {
                T item = Activator.CreateInstance<T>();
                foreach (DataColumn dc in dt.Columns)
                {
                    object val = dr[dc.ColumnName];
                    if (val == DBNull.Value) val = null;
                    PropertyInfo pi = item.GetType().GetProperty(dc.ColumnName);
                    val = Convert.ChangeType(val, pi.PropertyType);
                    if (pi != null)
                        pi.SetValue(item, val, null);
                }
                result.Add(item);
            }
            return result;
        }

        public DataSet ExecuteQuery(IDataFeed feed, ScriptParameters parameters)
        {
            DataSet ds = new DataSet();
            DataTable dt;

            string sql = feed.FeedQuery;
            string name = feed.FeedName;

            if (parameters != null)
                name = parameters.Replace(name);

            ds.DataSetName = name;

            if (feed.FeedType == DataFeedType.SQL)
            {
                var command = ReadOnlyCommand(CommandType.Text);

                if (parameters != null)
                {
                    sql = parameters.Replace(sql);
                    command.Param(parameters);
                }

                command.FillDataSet(ds, sql);

                if (ds.Tables.Count > 0)
                    ds.Tables[0].TableName = "default";
            }
            else
            {
                ScriptResult result = Provider.Data.Feed.ScriptEngine.Run(feed.FeedQuery, parameters);
                if (result.Exception != null)
                    throw result.Exception;
                foreach (var kvp in result.DataSet)
                {
                    dt = kvp.Value.AsDataTable();
                    dt.TableName = kvp.Key;
                    ds.Tables.Add(dt);
                }
            }

            return ds;
        }

        public static IList<DataTable> GetTables(DataSet ds, string key)
        {
            IList<DataTable> result = null;
            if (string.IsNullOrEmpty(key))
                result = ds.Tables.Cast<DataTable>().ToList();
            else
                result = ds.Tables.Cast<DataTable>().Where(x => x.TableName == key).ToList();
            return result;
        }

        public static void DeleteFeed(IDataFeed feed, IPrivileged client)
        {
            if (feed != null)
            {
                if (DataFeedItem.CanDeleteFeed(client))
                {
                    feed.FeedAlias += "$DELETED$" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    feed.Deleted = true;
                }
            }
        }

        public string CsvFeedContent(DataFeedResult feed, string key)
        {
            if (string.IsNullOrEmpty(key)) key = "default";

            var dt = feed.Data[key];

            StringBuilder sb = new StringBuilder();
            string comma = string.Empty;
            foreach (string k in dt.Keys())
            {
                sb.AppendFormat("{0}\"{1}\"", comma, k.Replace("\"", "\"\""));
                comma = ",";
            }
            sb.Append(Environment.NewLine);
            foreach (var dr in dt)
            {
                comma = string.Empty;
                foreach (string k in dr.Keys())
                {
                    sb.AppendFormat("{0}\"{1}\"", comma, dr[k].Replace("\"", "\"\""));
                    comma = ",";
                }
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        public string XmlFeedContent(DataFeedResult feed)
        {
            XmlDocument xdoc = new XmlDocument();

            xdoc.LoadXml("<?xml version=\"1.0\"?><data></data>");
            XmlNode data = xdoc.SelectSingleNode("/data");

            XmlAttribute attr;
            attr = xdoc.CreateAttribute("name");
            attr.Value = feed.Name;
            data.Attributes.Append(attr);

            if (feed.Data.Count > 0)
            {
                foreach (var kvp in feed.Data)
                {
                    XmlNode table = xdoc.CreateElement("table");
                    attr = xdoc.CreateAttribute("name");
                    attr.Value = kvp.Key;
                    table.Attributes.Append(attr);

                    foreach (var item in kvp.Value)
                    {
                        XmlNode row = xdoc.CreateElement("row");

                        foreach (var k in item.Keys())
                        {
                            XmlNode child = xdoc.CreateElement(k.Replace("/", "_"));
                            child.InnerText = item[k].ToString();
                            row.AppendChild(child);
                        }

                        table.AppendChild(row);
                    }

                    data.AppendChild(table);
                }
            }

            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            xdoc.WriteTo(xw);

            return sw.GetStringBuilder().ToString();
        }

        public string RssFeedContent(DataFeedResult feed, string key, Uri requestUri, string absolutePath)
        {
            if (string.IsNullOrEmpty(key)) key = "default";

            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?><rss version=\"2.0\"><channel></channel></rss>");
            XmlNode channel = xdoc.SelectSingleNode("/rss/channel");

            XmlNode child;
            child = xdoc.CreateElement("title");
            child.InnerText = feed.Name;
            channel.AppendChild(child);

            child = xdoc.CreateElement("description");
            if (string.IsNullOrEmpty(feed.Description))
                child.InnerText = $"{GlobalSettings.Current.CompanyName} On-Line Services Data Feed";
            else
                child.InnerText = feed.Description;
            channel.AppendChild(child);

            child = xdoc.CreateElement("link");
            child.InnerText = requestUri.GetLeftPart(UriPartial.Authority);
            channel.AppendChild(child);

            child = xdoc.CreateElement("lastBuildDate");
            child.InnerText = DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH':'mm':'ss '+0000'");
            channel.AppendChild(child);

            child = xdoc.CreateElement("pubDate");
            child.InnerText = DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH':'mm':'ss '+0000'");
            channel.AppendChild(child);

            var dt = feed.Data[key];

            int i = 0;
            foreach (var dr in dt)
            {
                XmlNode item = xdoc.CreateElement("item");

                child = xdoc.CreateElement("title");
                if (dr.Keys().Contains("Title"))
                    child.InnerText = dr["Title"].ToString();
                else
                    child.InnerText = "Missing \"Title\" field.";
                item.AppendChild(child);

                child = xdoc.CreateElement("description");
                if (dr.Keys().Contains("Description"))
                    child.InnerText = dr["Description"].ToString();
                else
                    child.InnerText = "Missing \"Description\" field.";
                item.AppendChild(child);

                child = xdoc.CreateElement("link");
                if (dr.Keys().Contains("Link"))
                    child.InnerText = dr["Link"].ToString();
                else
                    child.InnerText = "Missing \"Link\" field.";
                item.AppendChild(child);

                child = xdoc.CreateElement("guid");
                child.InnerText = DataFeedUtility.FeedItemURL(feed, "rss", requestUri, absolutePath) + "&i=" + i.ToString();
                item.AppendChild(child);

                child = xdoc.CreateElement("pubDate");
                if (dr.Keys().Contains("PubDate"))
                    child.InnerText = Convert.ToDateTime(dr["PubDate"]).ToString("ddd, dd MMM yyyy HH':'mm':'ss '+0000'");
                else
                    child.InnerText = DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH':'mm':'ss '+0000'");
                item.AppendChild(child);

                channel.AppendChild(item);

                i++;
            }

            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            xdoc.WriteTo(xw);

            return sw.GetStringBuilder().ToString();
        }

        public string HtmlFeedContent(DataFeedResult feed, string format = null)
        {
            bool fullPage = format != "table";

            //DataSet ds = ExecuteQuery(feed, parameters);
            //IList<DataTable> tables = GetTables(ds, key);

            StringBuilder sb = new StringBuilder();
            if (fullPage)
            {
                sb.AppendLine("<!DOCTYPE html>");
                sb.AppendLine("<html>");
                sb.AppendLine("<head>");
                sb.AppendLine($"<title>{GlobalSettings.Current.CompanyName} Feed</title>");
                sb.AppendLine("<style>");
                sb.AppendLine(".lnf-feed-container {margin: 10px;}");
                sb.AppendLine(".grid.lnf-feed {margin-bottom: 10px;}");
                sb.AppendLine("</style>");
                sb.AppendLine("</head>");
                sb.AppendLine("<body>");
            }
            sb.AppendLine("<div class=\"lnf-feed-container app\">");
            foreach (var dt in feed.Data)
            {
                sb.AppendLine("<table class=\"lnf-feed grid table\">");
                sb.AppendLine("<thead>");
                sb.AppendLine("<tr>");
                foreach (string k in dt.Value.Keys())
                {
                    sb.AppendLine($"<th>{k}</th>");
                }
                sb.AppendLine("</tr>");
                sb.AppendLine("</thead>");
                sb.AppendLine("<tbody>");
                foreach (var dr in dt.Value)
                {
                    sb.AppendLine("<tr>");
                    foreach (string k in dr.Keys())
                    {
                        sb.AppendLine($"<td>{dr[k]}</td>");
                    }
                    sb.AppendLine("</tr>");
                }
                sb.AppendLine("</tbody>");
                sb.AppendLine("</table>");
            }
            sb.AppendLine("</div>");
            if (fullPage)
            {
                sb.AppendLine("</body>");
                sb.AppendLine("</html>");
            }

            return sb.ToString();
        }

        public string JsonFeedContent(DataFeedResult feed, string key, string format = null)
        {
            object obj;

            Hashtable data = new Hashtable();

            foreach (var dt in feed.Data)
            {
                ArrayList table = new ArrayList();
                foreach (var dr in dt.Value)
                {
                    Hashtable hash = new Hashtable();
                    foreach (string k in dr.Keys())
                    {
                        hash.Add(k, dr[k]);
                    }
                    table.Add(hash);
                }

                data.Add(dt.Key, table);
            }

            if (format == "datatables")
            {
                if (string.IsNullOrEmpty(key))
                    obj = new { aaData = data["default"] };
                else
                    obj = new { aaData = data[key] };
            }
            else
            {
                obj = new
                {
                    feed.ID,
                    feed.GUID,
                    feed.Name,
                    feed.Private,
                    feed.Active,
                    Data = data
                };
            }

            string result = ServiceProvider.Current.Utility.Serialization.Json.Serialize(obj);

            return result;
        }

        public string IcalFeedContent(DataFeedResult feed, string key, string localAddr)
        {
            if (string.IsNullOrEmpty(key)) key = "default";

            var dt = feed.Data[key];

            StringBuilder sb = new StringBuilder();
            string serverIP = localAddr;
            DateTime buildTime = DateTime.UtcNow;
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("METHOD:PUBLISH");
            sb.AppendLine($"PRODID:-//{serverIP}//NONSGML {GlobalSettings.Current.CompanyName}-ICAL 1.0//");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("X-WR-CALNAME:" + feed.Name);
            sb.AppendLine($"X-WR-CALDESC:{GlobalSettings.Current.CompanyName} Online Services Data Feed");
            sb.AppendLine("X-WR-TIMEZONE:US-Eastern");
            int i = 0;
            foreach (var dr in dt)
            {
                /*
                BEGIN:VEVENT
                UID:20120320T165310CET-0068cUDLNK@192.168.100.199
                DTSTAMP:20120320T155310Z
                DESCRIPTION:http://www.aadl.org/catalog/record/1163159
                DTSTART;VALUE=DATE:20120329
                SUMMARY:DUE: Proof positive
                END:VEVENT
                */
                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine("UID:" + feed.Alias + "@" + serverIP);
                sb.AppendLine("DTSTAMP:" + buildTime.ToString("yyyyMMdd'T'HHmmss'Z'"));
                if (dt.Keys().Contains("DESCRIPTION"))
                    sb.AppendLine("DESCRIPTION:" + dr["DESCRIPTION"].ToString());
                if (dt.Keys().Contains("DTSTART"))
                    sb.AppendLine("DTSTART:" + Convert.ToDateTime(dr["DTSTART"]).ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                if (dt.Keys().Contains("DTSTART"))
                    sb.AppendLine("DTEND:" + Convert.ToDateTime(dr["DTSTART"]).ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                if (dt.Keys().Contains("SUMMARY"))
                    sb.AppendLine("SUMMARY:" + dr["SUMMARY"].ToString());
                sb.AppendLine("END:VEVENT");
                i++;
            }
            sb.Append("END:VCALENDAR");

            return sb.ToString();
        }

        public static string FeedItemURL(DataFeedResult feed, string format, Uri requestUri, string absolutePath)
        {
            string result = requestUri.GetLeftPart(UriPartial.Authority) + absolutePath + "/feed/";
            result += feed.Alias + "/";
            result += (string.IsNullOrEmpty(format) ? "xml" : format);
            return result;
        }
    }
}
