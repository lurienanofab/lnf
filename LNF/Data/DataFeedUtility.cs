using LNF.Cache;
using LNF.CommonTools;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Scripting;
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
    public static class DataFeedUtility
    {
        public static bool CanEditFeed()
        {
            return CacheManager.Current.CurrentUser.HasPriv(ClientPrivilege.Developer | ClientPrivilege.Administrator);
        }

        public static bool CanAddFeed()
        {
            return CacheManager.Current.CurrentUser.HasPriv(ClientPrivilege.Developer | ClientPrivilege.Administrator);
        }

        public static bool CanDeleteFeed()
        {
            return CacheManager.Current.CurrentUser.HasPriv(ClientPrivilege.Developer | ClientPrivilege.Administrator);
        }

        public static bool CanViewFeedList()
        {
            return true;
        }

        public static bool CanViewInactiveFeeds()
        {
            return CacheManager.Current.CurrentUser.HasPriv(ClientPrivilege.Developer | ClientPrivilege.Administrator);
        }

        public static DataTable GetDataTable(string alias, object parameters)
        {
            string query = GetQueryString(parameters);
            string url = string.Format("http://ssel-sched.eecs.umich.edu/data/feed/{0}/xml/{1}", alias, query);
            DataSet ds = new DataSet();
            ds.ReadXml(url);
            DataTable dt = ds.Tables[0];
            return dt;
        }

        public static string GetCSV(string alias, object parameters, string host = "ssel-sched.eecs.umich.edu")
        {
            string result = string.Empty;
            string query = GetQueryString(parameters);
            string url = string.Format("http://{0}/data/feed/{1}/csv/{2}", host, alias, query);
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

        public static string GetQueryString(object parameters)
        {
            string result = string.Empty;
            if (parameters != null)
            {
                IDictionary<string, object> dictionary = RepositoryUtility.ObjectToDictionary(parameters);
                string amp = "?";
                if (dictionary.Count > 0)
                {
                    foreach (KeyValuePair<string, object> kvp in dictionary)
                    {
                        result += amp + kvp.Key + "=" + kvp.Value.ToString();
                        amp = "&";
                    }
                }
            }
            return result;
        }

        public static DataSet ExecuteQuery(DataFeed feed, Parameters parameters = null)
        {
            DataSet ds = new DataSet();
            DataTable dt = null;

            string sql = feed.FeedQuery;
            string name = feed.FeedName;

            if (parameters != null)
                name = parameters.Replace(name);

            ds.DataSetName = name;

            if (feed.FeedType == DataFeedType.SQL)
            {
                using (SQLDBAccess dba = new SQLDBAccess("cnSselDataReadOnly"))
                {
                    if (parameters != null)
                    {
                        sql = parameters.Replace(sql);
                        dba.SelectCommand.ApplyParameters(parameters);
                    }
                    
                    dba.SelectCommand.CommandType = CommandType.Text;

                    ds = dba.FillDataSet(sql);

                    if (ds.Tables.Count > 0)
                        ds.Tables[0].TableName = "default";

                    //dt = dba.FillDataTable(sql, commandType: CommandType.Text);
                    //dt.TableName = "default";
                    //ds.Tables.Add(dt);
                }
            }
            else
            {
                Result result = Providers.Scripting.Run(feed.FeedQuery, parameters);
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

        public static void DeleteFeed(DataFeed feed)
        {
            if (feed != null)
            {
                if (DataFeedUtility.CanDeleteFeed())
                {
                    feed.FeedAlias += "$DELETED$" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    feed.Deleted = true;
                }
            }
        }

        public static string CsvFeedContent(DataFeed feed, string key, Parameters parameters = null)
        {
            if (string.IsNullOrEmpty(key)) key = "default";

            DataSet ds = ExecuteQuery(feed, parameters);
            DataTable dt = GetTables(ds, key).FirstOrDefault();

            StringBuilder sb = new StringBuilder();
            string comma = string.Empty;
            foreach (DataColumn dc in dt.Columns)
            {
                sb.AppendFormat("{0}\"{1}\"", comma, dc.ColumnName.Replace("\"", "\"\""));
                comma = ",";
            }
            sb.Append(Environment.NewLine);
            foreach (DataRow dr in dt.Rows)
            {
                comma = string.Empty;
                foreach (DataColumn dc in dt.Columns)
                {
                    sb.AppendFormat("{0}\"{1}\"", comma, dr[dc.ColumnName].ToString().Replace("\"", "\"\""));
                    comma = ",";
                }
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        public static string XmlFeedContent(DataFeed feed, string key, Parameters parameters = null)
        {
            DataSet ds = ExecuteQuery(feed, parameters);
            IList<DataTable> tables = GetTables(ds, key);

            XmlDocument xdoc = new XmlDocument();

            xdoc.LoadXml("<?xml version=\"1.0\"?><data></data>");
            XmlNode data = xdoc.SelectSingleNode("/data");

            XmlAttribute attr;
            attr = xdoc.CreateAttribute("name");
            attr.Value = ds.DataSetName;
            data.Attributes.Append(attr);

            if (tables.Count > 0)
            {
                foreach (DataTable dt in tables)
                {
                    XmlNode table = xdoc.CreateElement("table");
                    attr = xdoc.CreateAttribute("name");
                    attr.Value = dt.TableName;
                    table.Attributes.Append(attr);

                    foreach (DataRow dr in dt.Rows)
                    {
                        XmlNode row = xdoc.CreateElement("row");

                        foreach (DataColumn dc in dt.Columns)
                        {
                            XmlNode child = xdoc.CreateElement(dc.ColumnName.Replace("/", "_"));
                            child.InnerText = dr[dc.ColumnName].ToString();
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

        public static string RssFeedContent(DataFeed feed, string key, Parameters parameters = null)
        {
            if (string.IsNullOrEmpty(key)) key = "default";

            DataSet ds = ExecuteQuery(feed, parameters);
            DataTable dt = GetTables(ds, key).FirstOrDefault();

            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?><rss version=\"2.0\"><channel></channel></rss>");
            XmlNode channel = xdoc.SelectSingleNode("/rss/channel");

            XmlNode child;
            child = xdoc.CreateElement("title");
            child.InnerText = ds.DataSetName;
            channel.AppendChild(child);

            child = xdoc.CreateElement("description");
            if (string.IsNullOrEmpty(feed.FeedDescription))
                child.InnerText = "LNF On-Line Services Data Feed";
            else
                child.InnerText = feed.FeedDescription;
            channel.AppendChild(child);

            child = xdoc.CreateElement("link");
            child.InnerText = Providers.Context.Current.GetRequestUrl().GetLeftPart(UriPartial.Authority);
            channel.AppendChild(child);

            child = xdoc.CreateElement("lastBuildDate");
            child.InnerText = DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH':'mm':'ss '+0000'");
            channel.AppendChild(child);

            child = xdoc.CreateElement("pubDate");
            child.InnerText = DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH':'mm':'ss '+0000'");
            channel.AppendChild(child);

            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                XmlNode item = xdoc.CreateElement("item");

                child = xdoc.CreateElement("title");
                if (dt.Columns.Contains("Title"))
                    child.InnerText = dr["Title"].ToString();
                else
                    child.InnerText = "Missing \"Title\" field.";
                item.AppendChild(child);

                child = xdoc.CreateElement("description");
                if (dt.Columns.Contains("Description"))
                    child.InnerText = dr["Description"].ToString();
                else
                    child.InnerText = "Missing \"Description\" field.";
                item.AppendChild(child);

                child = xdoc.CreateElement("link");
                if (dt.Columns.Contains("Link"))
                    child.InnerText = dr["Link"].ToString();
                else
                    child.InnerText = "Missing \"Link\" field.";
                item.AppendChild(child);

                child = xdoc.CreateElement("guid");
                child.InnerText = DataFeedUtility.FeedItemURL(feed, "rss") + "&i=" + i.ToString();
                item.AppendChild(child);

                child = xdoc.CreateElement("pubDate");
                if (dt.Columns.Contains("PubDate"))
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

        public static string HtmlFeedContent(DataFeed feed, string key, string format = null, Parameters parameters = null)
        {
            bool fullPage = (format == "table") ? false : true;

            DataSet ds = ExecuteQuery(feed, parameters);
            IList<DataTable> tables = GetTables(ds, key);

            StringBuilder sb = new StringBuilder();
            if (fullPage)
            {
                sb.AppendLine("<!DOCTYPE html>");
                sb.AppendLine("<html>");
                sb.AppendLine("<head>");
                sb.AppendLine("<title>LNF Feed</title>");
                sb.AppendLine("<style>");
                sb.AppendLine(".lnf-feed-container {margin: 10px;}");
                sb.AppendLine(".grid.lnf-feed {margin-bottom: 10px;}");
                sb.AppendLine("</style>");
                sb.AppendLine("</head>");
                sb.AppendLine("<body>");
            }
            sb.AppendLine("<div class=\"lnf-feed-container app\">");
            foreach (DataTable dt in tables)
            {
                sb.AppendLine("<table class=\"lnf-feed grid table\">");
                sb.AppendLine("<thead>");
                sb.AppendLine("<tr>");
                foreach (DataColumn dc in dt.Columns)
                {
                    sb.AppendLine(string.Format("<th>{0}</th>", dc.ColumnName));
                }
                sb.AppendLine("</tr>");
                sb.AppendLine("</thead>");
                sb.AppendLine("<tbody>");
                foreach (DataRow dr in dt.Rows)
                {
                    sb.AppendLine("<tr>");
                    foreach (DataColumn dc in dt.Columns)
                    {
                        sb.AppendLine(string.Format("<td>{0}</td>", dr[dc.ColumnName].ToString()));
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

        public static string JsonFeedContent(DataFeed feed, string key, string format = null, Parameters parameters = null)
        {
            object obj = null;

            if (feed.FeedID != 0)
            {
                DataSet ds = ExecuteQuery(feed, parameters);
                IList<DataTable> tables = GetTables(ds, key);
                Hashtable data = new Hashtable();

                foreach (DataTable dt in tables)
                {
                    ArrayList table = new ArrayList();
                    foreach (DataRow dr in dt.Rows)
                    {
                        Hashtable hash = new Hashtable();
                        foreach (DataColumn dc in dt.Columns)
                        {
                            hash.Add(dc.ColumnName, dr[dc.ColumnName].ToString());
                        }
                        table.Add(hash);
                    }

                    data.Add(dt.TableName, table);
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
                        ID = feed.FeedID,
                        GUID = feed.FeedGUID,
                        Name = ds.DataSetName,
                        Private = feed.Private,
                        Active = feed.Active,
                        Data = data
                    };
                }
            }

            string result = Providers.Serialization.Json.Serialize(obj);

            return result;
        }

        public static string IcalFeedContent(DataFeed feed, string key, Parameters parameters = null)
        {
            if (string.IsNullOrEmpty(key)) key = "default";

            DataSet ds = ExecuteQuery(feed, parameters);
            DataTable dt = GetTables(ds, key).FirstOrDefault();

            StringBuilder sb = new StringBuilder();
            string serverIP = Providers.Context.Current.ServerVariables["LOCAL_ADDR"];
            DateTime buildTime = DateTime.UtcNow;
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("METHOD:PUBLISH");
            sb.AppendLine("PRODID:-//" + serverIP + "//NONSGML LNF-ICAL 1.0//");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("X-WR-CALNAME:" + ds.DataSetName);
            sb.AppendLine("X-WR-CALDESC:LNF On-Line Services Data Feed");
            sb.AppendLine("X-WR-TIMEZONE:US-Eastern");
            int i = 0;
            foreach (DataRow dr in dt.Rows)
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
                sb.AppendLine("UID:" + feed.FeedAlias + "@" + serverIP);
                sb.AppendLine("DTSTAMP:" + buildTime.ToString("yyyyMMdd'T'HHmmss'Z'"));
                if (dt.Columns.Contains("DESCRIPTION"))
                    sb.AppendLine("DESCRIPTION:" + dr["DESCRIPTION"].ToString());
                if (dt.Columns.Contains("DTSTART"))
                    sb.AppendLine("DTSTART:" + Convert.ToDateTime(dr["DTSTART"]).ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                if (dt.Columns.Contains("DTSTART"))
                    sb.AppendLine("DTEND:" + Convert.ToDateTime(dr["DTSTART"]).ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'"));
                if (dt.Columns.Contains("SUMMARY"))
                    sb.AppendLine("SUMMARY:" + dr["SUMMARY"].ToString());
                sb.AppendLine("END:VEVENT");
                i++;
            }
            sb.Append("END:VCALENDAR");

            return sb.ToString();
        }

        public static string FeedItemURL(DataFeed feed, string format)
        {
            string result = Providers.Context.Current.GetRequestUrl().GetLeftPart(UriPartial.Authority) + Providers.Context.Current.GetAbsolutePath("~") + "/feed/";
            result += feed.FeedAlias + "/";
            result += (string.IsNullOrEmpty(format) ? "xml" : format);
            return result;
        }
    }
}
