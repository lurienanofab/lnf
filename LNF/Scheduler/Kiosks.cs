using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Caching;

namespace LNF.Scheduler
{
    public class Kiosks
    {
        public IKioskRepository Repository { get; }

        private Kiosks(IKioskRepository repo)
        {
            Repository = repo;    
        }

        public static Kiosks Create(IKioskRepository repo)
        {
            return new Kiosks(repo);
        }

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
        public bool IsKiosk(string kioskIp)
        {
            // Check for local server
            if (kioskIp == "127.0.0.1")
                return true;

            // Specifically check for being on a kiosk - kiosks, resources, wagos are on the same subnet
            // If the user IP is on the kiosk list OR it starts with the right prefix defined by SchedulerProperty then they are on a kiosk
            if (kioskIp.StartsWith(Properties.Current.ResourceIPPrefix))
                return true;

            // check ips in web.config
            if (GetKiosksFromAppSettings().Contains(kioskIp))
                return true;

            // check ips in the database
            if (GetKiosks().Any(x => x.KioskIP == kioskIp))
                return true;

            return false;
        }

        /// <summary>
        /// Checks if on a kiosk based on ip (set in database), or if override is true (set in appSettings), or if ip is a defined kiosk (set in appSettings).
        /// </summary>
        public bool IsOnKiosk(string kioskIp)
        {
            if (OverrideIsOnKiosk) return true;
            if (IsKiosk(kioskIp)) return true;
            return false;
        }

        public IEnumerable<IKiosk> GetKiosks() => Repository.GetKiosks();

        public static string[] GetKiosksFromAppSettings()
        {
            var setting = ConfigurationManager.AppSettings["Kiosks"];
            if (string.IsNullOrEmpty(setting)) return new string[0];
            string[] result = setting.Split(',');
            return result;
        }

        public static string KioskRedirectUrl(string ip)
        {
            string json = File.ReadAllText("kiosks.json");

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
