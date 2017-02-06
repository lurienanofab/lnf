using LNF.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Scheduler
{
    public static class KioskUtility
    {
        public static bool IsKiosk()
        {
            if (Providers.Context.Current.GetSessionValue("IsKiosk") == null)
                Providers.Context.Current.SetSessionValue("IsKiosk", Providers.Context.Current.UserHostAddress.StartsWith("192.168.1"));
            object obj = Providers.Context.Current.GetSessionValue("IsKiosk");
            bool result = Convert.ToBoolean(obj);
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
    }
}
