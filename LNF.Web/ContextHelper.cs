using LNF.Data;
using LNF.Scheduler;
using LNF.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;

namespace LNF.Web
{
    public class ContextHelper
    {
        private static readonly object _lockingTarget = new object();

        public HttpContextBase Context { get; }
        public IProvider Provider { get; }

        public ContextHelper(HttpContextBase context, IProvider provider)
        {
            Context = context ?? throw new ArgumentNullException("context");
            Provider = provider ?? throw new ArgumentNullException("provider");
        }

        public IClient CurrentUser() => Context.CurrentUser(Provider);

        public IClient CheckSession() => Context.CheckSession(Provider);

        public void AppendLog(string msg)
        {
            List<SessionLogMessage> log;

            int clientId = 0;
            string username = "unknown";
            var currentUser = CurrentUser();
            if (currentUser != null)
            {
                clientId = currentUser.ClientID;
                username = currentUser.UserName;
            }

            DateTime now = DateTime.Now;
            SessionLogMessage logmsg;

            if (Context.Session["SessionLog"] == null)
            {
                logmsg = new SessionLogMessage { ClientID = clientId, UserName = username, LogDateTime = now, Text = "Started SessionLog" };
                log = new List<SessionLogMessage> { logmsg };
                AppendLogToFile(logmsg);

                Context.Session["SessionLog"] = log;
            }
            else
            {
                log = (List<SessionLogMessage>)Context.Session["SessionLog"];
            }

            if (!string.IsNullOrEmpty(msg))
            {
                logmsg = new SessionLogMessage { ClientID = clientId, UserName = username, LogDateTime = now, Text = msg };
                log.Add(logmsg);
                AppendLogToFile(logmsg);
            }
        }

        public IEnumerable<SessionLogMessage> GetLog()
        {
            // initalize the log if it doesn't already exist
            AppendLog(null);

            var log = (List<SessionLogMessage>)Context.Session["SessionLog"];

            return log;
        }

        public string GetLogText()
        {
            var log = GetLog();
            return string.Join(Environment.NewLine, log);
        }

        /// <summary>
        /// Checks if the kiosk ip begins with the ResourceIPPrefix (e.g. 192.168.1), or is a defined kiosk IP, or if Request.IsLocal is true. Does not check if user is in the lab.
        /// </summary>
        public bool IsKiosk()
        {
            bool result = Kiosks.Create(Provider.Scheduler.Kiosk).IsKiosk(Context.CurrentIP()) || Context.Request.IsLocal;
            return result;
        }

        /// <summary>
        /// Checks if on a kiosk based on ip (set in database), or if override is true (set in appSettings), or if ip is a defined kiosk (set in appSettings), of if Request.IsLocal is true.
        /// </summary>
        public bool IsOnKiosk()
        {
            bool result = Kiosks.Create(Provider.Scheduler.Kiosk).IsOnKiosk(Context.CurrentIP()) || Context.Request.IsLocal;
            return result;
        }

        private void AppendLogToFile(SessionLogMessage logmsg)
        {
            try
            {
                string securePath = ConfigurationManager.AppSettings["SecurePath"];
                string logSessionToFileSetting = ConfigurationManager.AppSettings["LogSessionToFile"];

                if (!string.IsNullOrEmpty(securePath) && !string.IsNullOrEmpty(logSessionToFileSetting))
                {
                    if (bool.TryParse(logSessionToFileSetting, out bool logSessionToFile))
                    {
                        if (logSessionToFile)
                        {
                            lock (_lockingTarget)
                            {
                                var logDir = Path.Combine(securePath, "logs", Provider.Log.Name);

                                if (!Directory.Exists(logDir))
                                    Directory.CreateDirectory(logDir);

                                var logFile = Path.Combine(logDir, "Session.log");

                                File.AppendAllText(logFile, logmsg.Message + Environment.NewLine);
                            }
                        }
                    }
                }
            }
            catch { } //silently fail, we don't want an error here to fuck everything up
        }
    }
}
