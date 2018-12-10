using LNF.Models.PhysicalAccess;
using LNF.Scheduler;
using System.Collections.Generic;
using System.Linq;

namespace LNF.PhysicalAccess
{
    public static class PhysicalAccessUtility
    {
        public static IEnumerable<Badge> CurrentlyInLab()
        {
            var result = ServiceProvider.Current.PhysicalAccess.GetCurrentlyInArea("all");
            return result;
        }

        public static bool IsInLab(int clientId)
        {
            return CurrentlyInLab().Any(x => x.ClientID == clientId);
        }

        /// <summary>
        /// Checks if the client is currently in any lab or is on a kiosk.
        /// </summary>
        public static bool ClientInLab(int clientId, string kioskIp)
        {
            if (IsInLab(clientId))
                return true;

            if (KioskUtility.IsOnKiosk(kioskIp))
                return true;

            // 2007-06-27 if it's SEM tool, we have to allow activation from any computer because there is no kiosk around that tool, this should be a temporary solution

            return false;
        }

        /// <summary>
        /// Checks if the client is currently in any lab, on a kiosk, or if the lab is an "always in" lab.
        /// </summary>
        public static bool ClientInLab(int clientId, string kioskIp, int labId)
        {
            // 2009-02-13 if this tools in DC lab, they can activate at anywhere

            // [2016-06-22 jg] all of these tool are in the same lab and there are no other tools in this lab, so it is better to just use
            //       the LabID. However, it is still terrible to hard code this, should be in the database or web.config at least.

            int[] alwaysInLabs = { 4 };
            if (alwaysInLabs.Contains(labId))
                return true;

            if (ClientInLab(clientId, kioskIp))
                return true;

            return false;
        }
    }
}
