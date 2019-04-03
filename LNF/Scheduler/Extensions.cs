using LNF.Cache;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Scheduler
{
    public static class CacheManagerExtensions
    {
        /// <summary>
        /// Gets all reservation activities. Activities are cached for 1 week.
        /// </summary>
        public static IEnumerable<ActivityItem> Activities(this CacheManager cm) => cm.GetValue("Activities", () => DA.Current.Query<Activity>().CreateModels<ActivityItem>(), DateTimeOffset.Now.Add(TimeSpan.FromDays(7)));

        public static ActivityItem GetActivity(this CacheManager cm, int activityId)
        {
            var result = cm.Activities().FirstOrDefault(x => x.ActivityID == activityId);

            if (result == null)
                throw new CacheItemNotFoundException<Activity>(x => x.ActivityID, activityId);

            return result;
        }

        public static IEnumerable<ActivityItem> AuthorizedActivities(this CacheManager cm, ClientAuthLevel authLevel)
        {
            //procActivitySelect @Action = 'SelectAuthorizedActivities'

            //SELECT ActivityID, ActivityName, ListOrder, Chargeable, Editable, AccountType, UserAuth, InviteeType, InviteeAuth,
            //    StartEndAuth, NoReservFenceAuth, NoMaxSchedAuth, [Description], IsActive, IsFacilityDownTime
            //FROM Activity
            //WHERE UserAuth &@UserAuth > 0
            //    AND Editable = 1 AND IsActive = 1
            //    AND IsFacilityDownTime = 0--This keeps the Facility Down Time activity out of the select when making reservations. Staff should use the link at the top of the page instead.
            //ORDER BY ListOrder

            var result = cm.Activities().Where(x => x.IsActive && (x.UserAuth & (int)authLevel) > 0 && x.Editable && !x.IsFacilityDownTime).OrderBy(x => x.ListOrder).ToList();

            return result;
        }

        /// <summary>
        /// Gets all scheduler properties. SchedulerProperties are cached for 1 week.
        /// </summary>
        public static IEnumerable<SchedulerPropertyItem> SchedulerProperties(this CacheManager cm) => cm.GetValue("SchedulerProperties", () => DA.Current.Query<SchedulerProperty>().CreateModels<SchedulerPropertyItem>(), DateTimeOffset.Now.AddDays(7));

        [Obsolete("Use HttpContextBase instead.")]
        public static ClientSetting GetClientSetting(this CacheManager cm)
        {
            var result = cm.GetContextItem<ClientSetting>(SessionKeys.ClientSetting);

            if (result == null)
            {
                result = ClientSetting.GetClientSettingOrDefault(cm.CurrentUser.ClientID);
                cm.SetContextItem(SessionKeys.ClientSetting, result);
            }

            return result;
        }

        public static IEnumerable<ResourceClientItem> ResourceClients(this CacheManager cm, int resourceId) => cm.GetValue($"ResourceClients#{resourceId}", () => DA.Current.Query<ResourceClientInfo>().Where(x => x.ResourceID == resourceId).CreateModels<ResourceClientItem>(), DateTimeOffset.Now.AddMinutes(5));

        public static IEnumerable<ResourceClientItem> ToolEngineers(this CacheManager cm, int resourceId) => cm.ResourceClients(resourceId).Where(x => (x.AuthLevel & ClientAuthLevel.ToolEngineer) > 0).ToList();

        public static ResourceClientItem GetResourceClient(this CacheManager cm, int resourceId, int clientId) => cm.ResourceClients(resourceId).FirstOrDefault(x => x.ClientID == clientId || x.ClientID == -1);

        public static ClientAuthLevel GetAuthLevel(this CacheManager cm, int resourceId, int clientId)
        {
            var client = cm.GetClient(clientId);
            var resourceClients = cm.ResourceClients(resourceId);
            return ReservationUtility.GetAuthLevel(resourceClients, client);
        }

        public static void ClearResourceClients(this CacheManager cm, int resourceId) => cm.RemoveValue($"ResourceClients#{resourceId}");

        public static IEnumerable<ProcessInfoItem> ProcessInfos(this CacheManager cm, int resourceId) => cm.GetValue($"ProcessInfos#{resourceId}", () => DA.Current.Query<ProcessInfo>().Where(x => x.Resource.ResourceID == resourceId).CreateModels<ProcessInfoItem>(), DateTimeOffset.Now.AddMinutes(5));

        public static IEnumerable<ProcessInfoLineItem> ProcessInfoLines(this CacheManager cm, int processInfoId) => cm.GetValue($"ProcessInfoLines#{processInfoId}", () => DA.Current.Query<ProcessInfoLine>().Where(x => x.ProcessInfoID == processInfoId).CreateModels<ProcessInfoLineItem>(), DateTimeOffset.Now.AddMinutes(5));

        /// <summary>
        /// Gets ResourceCosts for all ChargeTypes and Resources. Cached for 24 hours.
        /// </summary>
        public static IEnumerable<ResourceCost> ResourceCosts(this CacheManager cm) => cm.GetValue("ResourceCosts", () => ServiceProvider.Current.ResourceManager.GetResourceCosts(), DateTimeOffset.Now.AddHours(24));

        /// <summary>
        /// Gets a ResourceCost for each ChargeType for the given ResourceID.
        /// </summary>
        public static IEnumerable<ResourceCost> ResourceCosts(this CacheManager cm, int resourceId)
        {
            return cm.ResourceCosts().Where(x => x.ResourceID == resourceId);
        }

        /// <summary>
        /// Gets a single ResourceCost for given ResourceID and ChargeTypeID.
        /// </summary>
        public static ResourceCost GetResourceCost(this CacheManager cm, int resourceId, int chargeTypeId)
        {
            return cm.ResourceCosts(resourceId).FirstOrDefault(x => x.ChargeTypeID == chargeTypeId);
        }
    }
}
