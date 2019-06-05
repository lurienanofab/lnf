using System;
using System.Collections.Generic;

namespace LNF.Models.Data
{
    public enum DataFeedType
    {
        SQL = 0,
        Python = 1
    }

    public class DataFeedItem : IDataFeed
    {
        public int FeedID { get; set; }
        public Guid FeedGUID { get; set; }
        public string FeedAlias { get; set; }
        public string FeedName { get; set; }
        public string FeedQuery { get; set; }
        public string DefaultParameters { get; set; }
        public bool Private { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public string FeedDescription { get; set; }
        public string FeedLink { get; set; }
        public DataFeedType FeedType { get; set; }
        public void ApplyDefaultParameters(IDictionary<object, object> parameters) => ApplyDefaultParameters(DefaultParameters, parameters);

        public static void ApplyDefaultParameters(string defaultParams, IDictionary<object, object> parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            var defs = new Dictionary<object, object>();

            if (!string.IsNullOrEmpty(defaultParams))
            {
                var pairs = defaultParams.Split('&');
                foreach (var p in pairs)
                {
                    var parts = p.Split('=');
                    if (parts.Length == 2)
                        defs.Add(parts[0], GetParamValue(parts[1]));
                }
            }

            foreach (var kvp in defs)
            {
                if (!parameters.ContainsKey(kvp.Key))
                    parameters.Add(kvp.Key, GetParamValue(kvp.Value));
            }
        }

        public static object GetParamValue(object obj)
        {
            var s = obj.ToString();

            if (s == "{prev-period}")
                return FirstOfMonth(DateTime.Now).AddMonths(-1).ToString("yyyy-MM-dd");
            else if (s == "{this-period}")
                return FirstOfMonth(DateTime.Now).ToString("yyyy-MM-dd");
            else if (s == "{next-period}")
                return FirstOfMonth(DateTime.Now).AddMonths(1).ToString("yyyy-MM-dd");
            else
                return s;
        }

        public static DateTime FirstOfMonth(DateTime d)
        {
            return new DateTime(d.Year, d.Month, 1);
        }

        public static bool CanViewInactiveFeeds(IPrivileged client)
        {
            return client.HasPriv(ClientPrivilege.Developer | ClientPrivilege.Administrator);
        }

        public static bool CanEditFeed(IPrivileged client)
        {
            return client.HasPriv(ClientPrivilege.Developer | ClientPrivilege.Administrator);
        }

        public static bool CanAddFeed(IPrivileged client)
        {
            return client.HasPriv(ClientPrivilege.Developer | ClientPrivilege.Administrator);
        }

        public static bool CanDeleteFeed(IPrivileged client)
        {
            return client.HasPriv(ClientPrivilege.Developer | ClientPrivilege.Administrator);
        }

        public static bool CanViewFeedList()
        {
            return true;
        }
    }
}
