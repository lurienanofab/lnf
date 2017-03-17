using LNF.Repository;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace LNF.Scheduler
{
    public static class KioskUtility
    {
        public static bool OverrideIsOnKiosk
        {
            get
            {
                bool result = false;
                bool.TryParse(ConfigurationManager.AppSettings["OverrideIsOnKiosk"], out result);
                return result;
            }
        }

        /// <summary>
        /// Checks if the kiosk ip begins with the ResourceIPPrefix (e.g. 192.168.1).
        /// </summary>
        public static bool IsKiosk()
        {
            return IsKiosk(Providers.Context.Current.UserHostAddress);
        }

        /// <summary>
        /// Checks if the kiosk ip begins with the ResourceIPPrefix (e.g. 192.168.1).
        /// </summary>
        public static bool IsKiosk(string kioskIp)
        {
            // Specifically check for being on a kiosk - kiosks, resources, wagos are on the same subnet
            // If the user IP is on the kiosk list OR it starts with the right prefix defined by SchedulerProperty then they are on a kiosk
            string prefix = Properties.Current.ResourceIPPrefix;
            return kioskIp.StartsWith(prefix);
        }

        /// <summary>
        /// Checks if on a kiosk based on ip, or client is in any lab.
        /// </summary>
        public static bool IsOnKiosk(int clientId, string kioskIp)
        {
            if (IsKiosk(kioskIp)) return true;
            int[] clientLabs = IpCheck(clientId, kioskIp);
            return IsOnKiosk(clientLabs, kioskIp);
        }

        /// <summary>
        /// Checks if on a kiosk based on ip, or client is in any lab.
        /// </summary>
        public static bool IsOnKiosk(int[] clientLabs, string kioskIp)
        {
            if (OverrideIsOnKiosk) return true;
            if (IsKiosk(kioskIp)) return true;
            bool result = clientLabs.Length > 0 && clientLabs[0] > 0;
            return result;
        }

        public static string KioskRedirectUrl()
        {
            Dictionary<string, string> table = new Dictionary<string, string>();
            table.Add("192.168.1.14", "inventory");

            string ip = Providers.Context.Current.UserHostAddress;

            if (table.ContainsKey(ip))
                return table[ip];
            else
                return "sselscheduler";
        }

        public static int[] IpCheck(int clientId, string kioskIp)
        {
            using (var adap = DA.Current.GetAdapter())
            {
                adap.AddParameter("@Action", "IpCheck");
                adap.AddParameter("@ClientID", clientId);
                adap.AddParameter("@KioskIP", kioskIp);
                var dt = adap.FillDataTable("sselScheduler.dbo.procKioskSelect");
                return dt.AsEnumerable().Select(x => x.Field<int>("LabID")).ToArray();
            }
        }

        /// <summary>
        /// Checks if the client is currently in the specified lab. Will also return true if OverrideIsOnKiosk is true or the lab is an "always in" lab.
        /// </summary>
        public static bool ClientInLab(int labId, int clientId, string kioskIp)
        {
            int[] clientLabs = IpCheck(clientId, kioskIp);
            bool result = ClientInLab(clientLabs, labId);
            return result;
        }

        /// <summary>
        /// Checks if the client is currently in the specified lab. Will also return true if OverrideIsOnKiosk is true or the lab is an "always in" lab.
        /// </summary>
        public static bool ClientInLab(int[] clientLabs, int labId)
        {
            if (OverrideIsOnKiosk) return true;

            bool result = clientLabs.Contains(labId);

            //if (HttpContext.Current.Request.IsLocal)
            //    result = true;

            // 2007-06-27 if it's SEM tool, we have to allow activation from any computer because there is no kiosk around that tool, this should be a temporary solution
            // 2009-02-13 if this tools in DC lab, they can activate at anywhere

            // [2016-06-22 jg] all of these tool are in the same lab and there are no other tools in this lab, so it is better to just use
            //       the LabID. However, it is still terrible to hard code this, should be in the database or web.config at least.

            int[] alwaysInLabs = { 4 };
            result = result || alwaysInLabs.Contains(labId);

            return result;
        }
    }
}
