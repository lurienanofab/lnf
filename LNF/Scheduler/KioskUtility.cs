using LNF.Models.PhysicalAccess;
using LNF.Models.Scheduler;
using LNF.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.Caching;

namespace LNF.Scheduler
{
    public static class KioskUtility
    {
        private readonly static MemoryCache _cache = new MemoryCache("KioskCache");

        public static bool OverrideIsOnKiosk
        {
            get
            {
                bool.TryParse(ConfigurationManager.AppSettings["OverrideIsOnKiosk"], out bool result);
                return result;
            }
        }

        /// <summary>
        /// Checks if the kiosk ip begins with the ResourceIPPrefix (e.g. 192.168.1).
        /// </summary>
        public static bool IsKiosk(string kioskIp)
        {
            // Check for local server
            if (kioskIp == "127.0.0.1")
                return true;

            // Specifically check for being on a kiosk - kiosks, resources, wagos are on the same subnet
            // If the user IP is on the kiosk list OR it starts with the right prefix defined by SchedulerProperty then they are on a kiosk
            if (kioskIp.StartsWith(Properties.Current.ResourceIPPrefix))
                return true;

            // check ips in the database
            if (GetKiosks().Any(x => x.KioskIP == kioskIp))
                return true;

            return false;
        }

        /// <summary>
        /// Checks if on a kiosk based on ip.
        /// </summary>
        public static bool IsOnKiosk(string kioskIp)
        {
            if (OverrideIsOnKiosk) return true;
            if (IsKiosk(kioskIp)) return true;
            return false;
        }

        public static IEnumerable<KioskItem> GetKiosks()
        {
            if (!_cache.Contains("Kiosks"))
            {
                var dt = DA.Command(CommandType.Text)
                    .FillDataTable("SELECT k.KioskID, k.KioskName, k.KioskIP, ISNULL(klab.LabID, 0) AS LabID FROM sselScheduler.dbo.Kiosk k LEFT JOIN sselScheduler.dbo.KioskLab klab ON klab.KioskID = k.KioskID");

                var kiosks = dt.AsEnumerable().Select(x => new KioskItem()
                {
                    KioskID = x.Field<int>("KioskID"),
                    KioskName = x.Field<string>("KioskName"),
                    KioskIP = x.Field<string>("KioskIP"),
                    LabID = x.Field<int>("LabID")
                }).ToList();

                _cache.Add("Kiosks", kiosks, new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(60) });
            }

            var result = (List<KioskItem>)_cache["Kiosks"];

            return result;
        }

        public static void ClearCache()
        {
            _cache.Remove("Kiosks");
        }

        public static string KioskRedirectUrl()
        {
            Dictionary<string, string> table = new Dictionary<string, string>
            {
                { "192.168.1.14", "inventory" }
            };

            string ip = ServiceProvider.Current.Context.UserHostAddress;

            if (table.ContainsKey(ip))
                return table[ip];
            else
                return "sselscheduler";
        }
    }
}
