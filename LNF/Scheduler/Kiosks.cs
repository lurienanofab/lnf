using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;

namespace LNF.Scheduler
{
    public static class Kiosks
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
        /// Checks if the kiosk ip begins with the ResourceIPPrefix (e.g. 192.168.1), or is a defined kiosk IP. Does not check if user is in the lab.
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
        /// Checks if on a kiosk based on ip, or if override is true (set in web.config).
        /// </summary>
        public static bool IsOnKiosk(string kioskIp)
        {
            if (OverrideIsOnKiosk) return true;
            if (IsKiosk(kioskIp)) return true;
            return false;
        }

        public static IEnumerable<IKiosk> GetKiosks()
        {
            IEnumerable<IKiosk> kiosks;

            if (!_cache.Contains("Kiosks"))
            {
                kiosks = ServiceProvider.Current.Scheduler.Kiosk.GetKiosks();
                _cache.Add("Kiosks", kiosks, new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(60) });
            }
            else
            {
                kiosks = (IEnumerable<IKiosk>)_cache["Kiosks"];
            }

            return kiosks;
        }

        public static void ClearCache()
        {
            _cache.Remove("Kiosks");
        }

        public static string KioskRedirectUrl(string ip)
        {
            Dictionary<string, string> table = new Dictionary<string, string>
            {
                { "192.168.1.14", "inventory" }
            };

            if (table.ContainsKey(ip))
                return table[ip];
            else
                return "sselscheduler";
        }
    }
}
