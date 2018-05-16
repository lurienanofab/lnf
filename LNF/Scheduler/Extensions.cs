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
                var items = DA.Current.Query<ResourceTreeItem>().Where(x => x.ClientID == cm.ClientID).ToList();
                result = new ResourceTreeItemCollection(items);
                cm.SetContextItem("ResourceTree", result);
            }

            return result;
        }

        /// <summary>
        /// Gets all reservation activities from cache. Cached for one request.
        /// </summary>
        public static IEnumerable<ActivityModel> Activities(this CacheManager cm)
        {
            var result = cm.GetContextItem<IEnumerable<ActivityModel>>("Activities");

            if (result == null || result.Count() == 0)
            {
                result = DA.Current.Query<Activity>().Model<ActivityModel>();
                cm.SetContextItem("Activities", result);
            }

            return result;
        }

        public static ActivityModel GetActivity(this CacheManager cm, int activityId)
        {
            var result = cm.Activities().FirstOrDefault(x => x.ActivityID == activityId);

            if (result == null)
                throw new CacheItemNotFoundException<Activity>(x => x.ActivityID, activityId);

            return result;
        }

        public static IEnumerable<ActivityModel> AuthorizedActivities(this CacheManager cm, ClientAuthLevel authLevel)
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

        public static ClientSetting GetClientSetting(this CacheManager cm) => cm.GetSessionValue(SessionKeys.ClientSetting, () => ClientSetting.GetClientSettingOrDefault(cm.ClientID));

        public static IEnumerable<ResourceClientModel> CurrentResourceClients(this CacheManager cm)
        {
            var result = cm.GetSessionValue(SessionKeys.CurrentResourceClients, () =>
            {
                int clientId = cm.ClientID;
                var query = DA.Current.Query<ResourceClientInfo>().Where(x => x.ClientID == clientId || x.ClientID == -1);
                var models = query.Model<ResourceClientModel>();
                return models;
            });

            return result;
        }

        public static ResourceClientModel GetCurrentResourceClient(this CacheManager cm, int resourceId) => cm.CurrentResourceClients().FirstOrDefault(x => x.ResourceID == resourceId);

        public static IEnumerable<ResourceClientModel> ResourceClients(this CacheManager cm, int resourceId)
        {
            string key = "ResourceClients#" + resourceId;

            var result = cm.GetContextItem<IEnumerable<ResourceClientModel>>(key);

            if (result == null || result.Count() <= 0)
            {
                result = DA.Current.Query<ResourceClientInfo>().Where(x => x.ResourceID == resourceId).Model<ResourceClientModel>();
                cm.SetContextItem(key, result);
            }

            return result;
        }

        public static IEnumerable<ResourceClientModel> ToolEngineers(this CacheManager cm, int resourceId) => cm.ResourceClients(resourceId).Where(x => (x.AuthLevel & ClientAuthLevel.ToolEngineer) > 0).ToList();

        public static ResourceClientModel GetResourceClient(this CacheManager cm, int resourceId, int clientId) => cm.ResourceClients(resourceId).FirstOrDefault(x => x.ClientID == clientId || x.ClientID == -1);

        public static ClientAuthLevel GetAuthLevel(this CacheManager cm, int resourceId, int clientId)
        {
            var client = cm.GetClient(clientId);
            var resourceClients = cm.ResourceClients(resourceId);
            return DA.Use<IReservationManager>().GetAuthLevel(resourceClients, client);
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
                result = DateTime.Now.Date;
                cm.WeekStartDate(result);
            }

            return result;
        }

        public static void WeekStartDate(this CacheManager cm, DateTime value) => cm.SetSessionValue(SessionKeys.WeekStartDate, value);

        public static bool IsOnKiosk(this CacheManager cm)
        {
            return cm.GetSessionValue(SessionKeys.IsOnKiosk, () =>
            {
                string kioskIp = ServiceProvider.Current.Context.UserHostAddress;
                return KioskUtility.IsOnKiosk(cm.GetClientLabs(), kioskIp);
            });
        }

        public static bool ClientInLab(this CacheManager cm, int labId) => KioskUtility.ClientInLab(cm.GetClientLabs(), labId);

        public static int[] GetClientLabs(this CacheManager cm)
        {
            return cm.GetSessionValue(SessionKeys.ClientLabs, () =>
            {
                string kioskIp = ServiceProvider.Current.Context.UserHostAddress;
                return KioskUtility.IpCheck(cm.ClientID, kioskIp);
            });
        }

        public static bool DisplayDefaultHours(this CacheManager cm) => cm.GetSessionValue(SessionKeys.DisplayDefaultHours, () => true);

        public static void DisplayDefaultHours(this CacheManager cm, bool value) => cm.SetSessionValue(SessionKeys.DisplayDefaultHours, value);

        public static IEnumerable<ProcessInfoModel> ProcessInfos(this CacheManager cm, int resourceId)
        {
            string key = "ProcessInfos#" + resourceId.ToString();

            var result = cm.GetContextItem<IEnumerable<ProcessInfoModel>>(key);

            if (result == null || result.Count() == 0)
            {
                result = DA.Current.Query<ProcessInfo>().Where(x => x.Resource.ResourceID == resourceId).Model<ProcessInfoModel>();
                cm.SetContextItem(key, result);
            }

            return result;
        }

        public static IEnumerable<ProcessInfoLineModel> ProcessInfoLines(this CacheManager cm, int processInfoId)
        {
            string key = "ProcessInfoLines#" + processInfoId.ToString();

            var result = cm.GetContextItem<IEnumerable<ProcessInfoLineModel>>(key);

            if (result == null || result.Count() == 0)
            {
                result = DA.Current.Query<ProcessInfoLine>().Where(x => x.ProcessInfoID == processInfoId).Model<ProcessInfoLineModel>();
                cm.SetContextItem(key, result);
            }

            return result;
        }

        public static IEnumerable<ReservationProcessInfoItem> ReservationProcessInfos(this CacheManager cm) => cm.GetSessionValue(SessionKeys.ReservationProcessInfos, () => new List<ReservationProcessInfoItem>());

        public static void ReservationProcessInfos(this CacheManager cm, IEnumerable<ReservationProcessInfoItem> value) => cm.SetSessionValue(SessionKeys.ReservationProcessInfos, value);

        public static IEnumerable<ReservationInviteeItem> ReservationInvitees(this CacheManager cm) => cm.GetSessionValue<IEnumerable<ReservationInviteeItem>>(SessionKeys.ReservationInvitees, () => new List<ReservationInviteeItem>());

        public static void ReservationInvitees(this CacheManager cm, IEnumerable<ReservationInviteeItem> value) => cm.SetSessionValue(SessionKeys.ReservationInvitees, value);

        public static IEnumerable<ReservationInviteeItem> RemovedInvitees(this CacheManager cm) => cm.GetSessionValue<IEnumerable<ReservationInviteeItem>>(SessionKeys.RemovedInvitees, () => new List<ReservationInviteeItem>());

        public static void RemovedInvitees(this CacheManager cm, IEnumerable<ReservationInviteeItem> value) => cm.SetSessionValue(SessionKeys.RemovedInvitees, value);

        public static IEnumerable<AvailableInviteeItem> AvailableInvitees(this CacheManager cm) => cm.GetSessionValue<IEnumerable<AvailableInviteeItem>>(SessionKeys.AvailableInvitees, () => new List<AvailableInviteeItem>());

        public static void AvailableInvitees(this CacheManager cm, IEnumerable<AvailableInviteeItem> value) => cm.SetSessionValue(SessionKeys.AvailableInvitees, value);

        public static ViewType CurrentViewType(this CacheManager cm) => cm.GetSessionValue("CurrentViewType", () => ViewType.WeekView);

        public static void CurrentViewType(this CacheManager cm, ViewType value) => cm.SetSessionValue("CurrentViewType", value);

        public static ClientAuthLevel SelectAuthLevel(this CacheManager cm, ResourceModel item, IPrivileged client) => cm.GetAuthLevel(item.ResourceID, client.ClientID);
    }
}
