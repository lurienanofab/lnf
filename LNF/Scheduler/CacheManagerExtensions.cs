using LNF.Cache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public static class CacheManagerExtensions
    {
        /// <summary>
        /// Gets all reservation activities. Activities are cached for 1 week.
        /// </summary>
        public static IEnumerable<IActivity> Activities(this ICache c) => c.GetValue("Activities", p => p.Scheduler.Activity.GetActivities(), DateTimeOffset.Now.Add(TimeSpan.FromDays(7)));

        public static IActivity GetActivity(this ICache c, int activityId)
        {
            var result = c.Activities().FirstOrDefault(x => x.ActivityID == activityId);

            if (result == null)
                throw new CacheItemNotFoundException<IActivity>(x => x.ActivityID, activityId);

            return result;
        }

        public static IEnumerable<IActivity> AuthorizedActivities(this ICache c, ClientAuthLevel authLevel)
        {
            //procActivitySelect @Action = 'SelectAuthorizedActivities'

            //SELECT ActivityID, ActivityName, ListOrder, Chargeable, Editable, AccountType, UserAuth, InviteeType, InviteeAuth,
            //    StartEndAuth, NoReservFenceAuth, NoMaxSchedAuth, [Description], IsActive, IsFacilityDownTime
            //FROM Activity
            //WHERE UserAuth &@UserAuth > 0
            //    AND Editable = 1 AND IsActive = 1
            //    AND IsFacilityDownTime = 0--This keeps the Facility Down Time activity out of the select when making reservations. Staff should use the link at the top of the page instead.
            //ORDER BY ListOrder

            var result = c.Activities().Where(x => x.IsActive && (x.UserAuth & (int)authLevel) > 0 && x.Editable && !x.IsFacilityDownTime).OrderBy(x => x.ListOrder).ToList();

            return result;
        }

        /// <summary>
        /// Gets all scheduler properties. SchedulerProperties are cached for 1 week.
        /// </summary>
        public static IEnumerable<ISchedulerProperty> SchedulerProperties(this ICache c) => c.GetValue("SchedulerProperties", p => p.Scheduler.Properties.GetSchedulerProperties(), DateTimeOffset.Now.AddDays(7));

        [Obsolete("Use HttpContextBase instead.")]
        public static IClientSetting GetClientSetting(this ICache c, int currentUserClientId)
        {
            throw new NotImplementedException();

            //var result = c.GetContextItem<IClientSetting>(SessionKeys.ClientSetting);

            //if (result == null)
            //{
            //    result = ServiceProvider.Current.Scheduler.ClientSetting.GetClientSettingOrDefault(c.CurrentUser.ClientID);
            //    c.SetContextItem(SessionKeys.ClientSetting, result);
            //}

            //return result;
        }

        public static IEnumerable<IResourceClient> ResourceClients(this ICache c, int resourceId) => c.GetValue($"ResourceClients#{resourceId}", p => p.Scheduler.Resource.GetResourceClients(resourceId), DateTimeOffset.Now.AddMinutes(5));

        public static IEnumerable<IResourceClient> ToolEngineers(this ICache c, int resourceId) => c.ResourceClients(resourceId).Where(x => (x.AuthLevel & ClientAuthLevel.ToolEngineer) > 0).ToList();

        public static IResourceClient GetResourceClient(this ICache c, int resourceId, int clientId) => c.ResourceClients(resourceId).GetResourceClient(clientId);

        public static IResourceClient GetResourceClient(this IEnumerable<IResourceClient> items, int clientId) => items.FirstOrDefault(x => x.IsClientOrEveryone(clientId));

        public static bool IsClientOrEveryone(this IAuthorized a, int clientId)
        {
            return a.ClientID == clientId || a.ClientID == -1;
        }

        public static ClientAuthLevel GetAuthLevel(this ICache c, int resourceId, int clientId)
        {
            var client = c.GetClient(clientId);
            var resourceClients = c.ResourceClients(resourceId);
            return Reservations.GetAuthLevel(resourceClients, client);
        }

        public static void ClearResourceClients(this ICache c, int resourceId) => c.RemoveValue($"ResourceClients#{resourceId}");

        public static IEnumerable<IProcessInfo> ProcessInfos(this ICache c, int resourceId) => c.GetValue($"ProcessInfos#{resourceId}", p => p.Scheduler.ProcessInfo.GetProcessInfos(resourceId), DateTimeOffset.Now.AddMinutes(5));

        public static IEnumerable<IProcessInfoLine> ProcessInfoLines(this ICache c, int resourceId, int processInfoId) => c.GetValue($"ProcessInfoLines#{processInfoId}", p => p.Scheduler.ProcessInfo.GetProcessInfoLines(resourceId).Where(x => x.ProcessInfoID == processInfoId), DateTimeOffset.Now.AddMinutes(5));

        /// <summary>
        /// Gets ResourceCosts for all ChargeTypes and Resources. Cached for 24 hours.
        /// </summary>
        public static IEnumerable<IResourceCost> ResourceCosts(this ICache c) => c.GetValue("ResourceCosts", p => p.Scheduler.Resource.GetResourceCosts(), DateTimeOffset.Now.AddHours(24));

        /// <summary>
        /// Gets a ResourceCost for each ChargeType for the given ResourceID.
        /// </summary>
        public static IEnumerable<IResourceCost> ResourceCosts(this ICache c, int resourceId)
        {
            return c.ResourceCosts().Where(x => x.ResourceID == resourceId);
        }

        /// <summary>
        /// Gets a single ResourceCost for given ResourceID and ChargeTypeID.
        /// </summary>
        public static IResourceCost GetResourceCost(this ICache c, int resourceId, int chargeTypeId)
        {
            return c.ResourceCosts(resourceId).FirstOrDefault(x => x.ChargeTypeID == chargeTypeId);
        }

        public static ILab GetLab(this ICache c, string name)
        {
            var labs = c.GetValue("Labs", p => p.Scheduler.Resource.GetLabs(), DateTimeOffset.Now.AddMinutes(30));
            return labs.FirstOrDefault(x => x.LabName == name || x.LabDisplayName == name);
        }
    }
}
