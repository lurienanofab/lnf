using LNF.Scheduler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace LNF.PhysicalAccess
{
    public class PhysicalAccessUtility
    {
        private int[] GetAlwaysInLabs()
        {
            string setting = ConfigurationManager.AppSettings["AlwaysInLabs"];

            if (string.IsNullOrEmpty(setting))
                throw new Exception("Missing required appSetting: AlwaysInLabs");

            int[] result = setting.Split(',').Select(int.Parse).ToArray();

            return result;
        }

        public bool IsOnKiosk { get; }
        public IEnumerable<Badge> CurrentlyInLab { get; }
        
        public PhysicalAccessUtility(IEnumerable<Badge> inlab, string kioskIp)
        {
            IsOnKiosk = Kiosks.IsOnKiosk(kioskIp);
            CurrentlyInLab = inlab;
        }

        /// <summary>
        /// Checks if the client is physically in the lab.
        /// </summary>
        public bool IsInLab(int clientId)
        {
            return CurrentlyInLab.Any(x => x.ClientID == clientId);
        }

        /// <summary>
        /// Checks if the client is currently in any lab or on a kiosk.
        /// </summary>
        public bool ClientInLab(int clientId)
        {
            if (IsInLab(clientId))
                return true;

            if (IsOnKiosk)
                return true;

            // 2007-06-27 if it's SEM tool, we have to allow activation from any computer because there is no kiosk around that tool, this should be a temporary solution

            return false;
        }

        /// <summary>
        /// Checks if the client is currently in any lab, on a kiosk, or if the lab is an "always in" lab.
        /// </summary>
        public bool ClientInLab(int clientId, int labId)
        {
            // 2009-02-13 if this tools in DC lab, they can activate at anywhere

            // [2016-06-22 jg] all of these tool are in the same lab and there are no other tools in this lab, so it is better to just use
            //       the LabID. However, it is still terrible to hard code this, should be in the database or web.config at least.

            if (GetAlwaysInLabs().Contains(labId))
                return true;

            if (ClientInLab(clientId))
                return true;

            return false;
        }
    }
}
