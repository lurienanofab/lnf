using LNF.Cache;
using LNF.Models.Data;
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
        public static ResourceTreeItemCollection ResourceTree(this CacheManager cm)
        {
            // always for the current user

            var result = cm.GetContextItem<ResourceTreeItemCollection>("ResourceTree");

            if (result == null || result.Count == 0)
            {
                var currentUserClientId = cm.CurrentUser.ClientID;
                var items = DA.Current.Query<ResourceTree>().Where(x => x.ClientID == currentUserClientId).Model<ResourceTreeItem>();
                result = new ResourceTreeItemCollection(items);
                cm.SetContextItem("ResourceTree", result);
            }

            return result;
        }

        /// <summary>
        /// Gets all reservation activities. Activities are cached for 1 week.
        /// </summary>
        public static IEnumerable<ActivityItem> Activities(this CacheManager cm)
        {
            IList<ActivityItem> result;

            var value = cm.GetMemoryCacheValue("Activities");

            if (value == null)
            {
                result = DA.Current.Query<Activity>().Model<ActivityItem>();
                cm.SetMemoryCacheValue("Activities", result, DateTimeOffset.Now.Add(TimeSpan.FromDays(7)));
            }
            else
            {
                result = (IList<ActivityItem>)value;
            }

            return result;
        }

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
        public static IEnumerable<SchedulerPropertyItem> SchedulerProperties(this CacheManager cm)
        {
            IList<SchedulerPropertyItem> result;

            var value = cm.GetMemoryCacheValue("SchedulerProperties");

            if (value == null)
            {
                result = DA.Current.Query<SchedulerProperty>().Model<SchedulerPropertyItem>();
                cm.SetMemoryCacheValue("SchedulerProperties", result, DateTimeOffset.Now.Add(TimeSpan.FromDays(7)));
            }
            else
            {
                result = (IList<SchedulerPropertyItem>)value;
            }

            return result;
        }

        public static ClientSetting GetClientSetting(this CacheManager cm) => cm.GetSessionValue(SessionKeys.ClientSetting, () => ClientSetting.GetClientSettingOrDefault(cm.CurrentUser.ClientID));

        public static ResourceClientItem GetCurrentResourceClient(this CacheManager cm, int resourceId)
        {
            // will return Everyone user if available because of the OrderBy
            return cm.ResourceClients(resourceId).OrderBy(x => x.ClientID).FirstOrDefault(x => x.ClientID == cm.CurrentUser.ClientID || x.ClientID == -1);
        }

        public static IEnumerable<ResourceClientItem> ResourceClients(this CacheManager cm, int resourceId)
        {
            string key = "ResourceClients#" + resourceId;

            var result = cm.GetContextItem<IEnumerable<ResourceClientItem>>(key);

            if (result == null || result.Count() <= 0)
            {
                result = DA.Current.Query<ResourceClientInfo>().Where(x => x.ResourceID == resourceId).Model<ResourceClientItem>();
                cm.SetContextItem(key, result);
            }

            return result;
        }

        public static IEnumerable<ResourceClientItem> ToolEngineers(this CacheManager cm, int resourceId) => cm.ResourceClients(resourceId).Where(x => (x.AuthLevel & ClientAuthLevel.ToolEngineer) > 0).ToList();

        public static ResourceClientItem GetResourceClient(this CacheManager cm, int resourceId, int clientId) => cm.ResourceClients(resourceId).FirstOrDefault(x => x.ClientID == clientId || x.ClientID == -1);

        public static ClientAuthLevel GetAuthLevel(this CacheManager cm, int resourceId, int clientId)
        {
            var client = cm.GetClient(clientId);
            var resourceClients = cm.ResourceClients(resourceId);
            return ServiceProvider.Current.Use<IReservationManager>().GetAuthLevel(resourceClients, client);
        }

        public static void ClearResourceClients(this CacheManager cm, int resourceId)
        {
            string key = "ResourceClients#" + resourceId.ToString();
            cm.RemoveContextItem(key);
        }

        public static DateTime WeekStartDate(this CacheManager cm)
        {
            var result = cm.GetSessionValue(SessionKeys.WeekStartDate, () => DateTime.Now.Date);

            if (result < Reservation.MinReservationBeginDate)
            {
                if (!DateTime.TryParse(ServiceProvider.Current.Context.QueryString["Date"], out result))
                    result = DateTime.Now.Date;
                cm.WeekStartDate(result);
            }

            return result;
        }

        public static void WeekStartDate(this CacheManager cm, DateTime value) => cm.SetSessionValue(SessionKeys.WeekStartDate, value);

        public static bool DisplayDefaultHours(this CacheManager cm) => cm.GetSessionValue(SessionKeys.DisplayDefaultHours, () => true);

        public static void DisplayDefaultHours(this CacheManager cm, bool value) => cm.SetSessionValue(SessionKeys.DisplayDefaultHours, value);

        [Obsolete]
        public static IEnumerable<ProcessInfoItem> ProcessInfos(this CacheManager cm, int resourceId)
        {
            string key = "ProcessInfos#" + resourceId.ToString();

            var result = cm.GetContextItem<IEnumerable<ProcessInfoItem>>(key);

            if (result == null || result.Count() == 0)
            {
                result = DA.Current.Query<ProcessInfo>().Where(x => x.Resource.ResourceID == resourceId).Model<ProcessInfoItem>();
                cm.SetContextItem(key, result);
            }

            return result;
        }

        [Obsolete]
        public static IEnumerable<ProcessInfoLineItem> ProcessInfoLines(this CacheManager cm, int processInfoId)
        {
            string key = "ProcessInfoLines#" + processInfoId.ToString();

            var result = cm.GetContextItem<IEnumerable<ProcessInfoLineItem>>(key);

            if (result == null || result.Count() == 0)
            {
                result = DA.Current.Query<ProcessInfoLine>().Where(x => x.ProcessInfoID == processInfoId).Model<ProcessInfoLineItem>();
                cm.SetContextItem(key, result);
            }

            return result;
        }

        /// <summary>
        /// Gets ResourceCosts for all ChargeTypes and Resources. Cached for 24 hours.
        /// </summary>
        public static IEnumerable<ResourceCost> ResourceCosts(this CacheManager cm)
        {
            var result = (IEnumerable<ResourceCost>)cm.GetMemoryCacheValue("ResourceCosts");

            if (result == null || result.Count() == 0)
            {
                result = ServiceProvider.Current.Use<IResourceManager>().GetResourceCosts();
                cm.SetMemoryCacheValue("ResourceCosts", result, DateTimeOffset.Now.AddHours(24));
            }

            return result;
        }

        /// <summary>
        /// Gets a ResourceCost for each ChargeType for the given ResourceID.
        /// </summary>
        public static IEnumerable<ResourceCost> ResourceCosts(this CacheManager cm, int resourceId)
        {
            return cm.ResourceCosts().Where(x => x.ResourceID == resourceId);
        }

        /// <summary>
        /// Gets a single ResourceCost for given ResourceID and the MaxChargeTypeID of the current user.
        /// </summary>
        public static ResourceCost GetResourceCost(this CacheManager cm, int resourceId)
        {
            return cm.GetResourceCost(resourceId, cm.CurrentUser.MaxChargeTypeID);
        }

        /// <summary>
        /// Gets a single ResourceCost for given ResourceID and ChargeTypeID.
        /// </summary>
        public static ResourceCost GetResourceCost(this CacheManager cm, int resourceId, int chargeTypeId)
        {
            return cm.ResourceCosts(resourceId).FirstOrDefault(x => x.ChargeTypeID == chargeTypeId);
        }

        [Obsolete]
        public static IEnumerable<ReservationProcessInfoItem> ReservationProcessInfos(this CacheManager cm) => cm.GetSessionValue(SessionKeys.ReservationProcessInfos, () => new List<ReservationProcessInfoItem>());

        [Obsolete]
        public static void ReservationProcessInfos(this CacheManager cm, IEnumerable<ReservationProcessInfoItem> value) => cm.SetSessionValue(SessionKeys.ReservationProcessInfos, value);

        public static ViewType CurrentViewType(this CacheManager cm) => cm.GetSessionValue("CurrentViewType", () => ViewType.WeekView);

        public static void CurrentViewType(this CacheManager cm, ViewType value) => cm.SetSessionValue("CurrentViewType", value);

        public static ClientAuthLevel SelectAuthLevel(this CacheManager cm, ResourceItem item, IPrivileged client) => cm.GetAuthLevel(item.ResourceID, client.ClientID);
    }
}
