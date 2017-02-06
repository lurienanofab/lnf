using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace LNF.Web
{
    public static class RequestLog
    {
        public static void Start()
        {
            var log = new List<string>();
            DateTime now = DateTime.Now;
            HttpContext.Current.Items["RequestLog.StartTime"] = now;
            log.Add(string.Format("[{0:yyyy-MM-dd HH:mm:ss}] Begin request {1}", now, HttpContext.Current.Request.Url.PathAndQuery));
            HttpContext.Current.Items["RequestLog"] = log;
        }

        public static void Append(string value, params object[] args)
        {
            var log = GetLog();

            if (log == null) return;

            string line = string.Format(value, args);

            log.Add(string.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1}", DateTime.Now, line));
        }

        public static string Flush()
        {
            TimeSpan timeTaken = DateTime.Now - GetStartTime();
            Append("Request complete in {0}", timeTaken);

            var log = GetLog();

            if (log == null)
                return string.Empty;

            HttpContext.Current.Items["RequestLog"] = null;

            return string.Join(Environment.NewLine, log);
        }

        public static string FlushHtml()
        {
            TimeSpan timeTaken = DateTime.Now - GetStartTime();
            Append("Request complete in {0}", timeTaken);

            var log = GetLog();

            if (log == null)
                return string.Empty;

            HttpContext.Current.Items["RequestLog"] = null;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<div class=\"request-log\">");
            foreach(string line in log)
            {
                sb.AppendLine(string.Format("<div class=\"log-entry\">{0}</div>", line));
            }

            sb.AppendLine("</div>");

            return sb.ToString();
        }

        public static string FlushScript()
        {
            TimeSpan timeTaken = DateTime.Now - GetStartTime();
            Append("Request complete in {0}", timeTaken);

            var log = GetLog();

            if (log == null)
                return string.Empty;

            HttpContext.Current.Items["RequestLog"] = null;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<script>");
            foreach (string line in log)
            {
                sb.AppendLine(string.Format("console.log('{0}');", line));
            }
            sb.AppendLine("</script>");

            return sb.ToString();
        }

        private static List<string> GetLog()
        {
            return (List<string>)HttpContext.Current.Items["RequestLog"];
        }

        private static DateTime GetStartTime()
        {
            if (HttpContext.Current.Items["RequestLog.StartTime"] == null)
                return default(DateTime);

            var result = (DateTime)HttpContext.Current.Items["RequestLog.StartTime"];

            return result;
        }
    }
}
