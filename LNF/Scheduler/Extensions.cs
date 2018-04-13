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
                var items = cm.ServiceProvider.DataAccess.Session.Query<ResourceTreeItem>().Where(x => x.ClientID == cm.ClientID).ToList();
                result = new ResourceTreeItemCollection(items);
                cm.SetContextItem("ResourceTree", result);
            }

            return result;
        }

        /// <summary>
        /// Gets all reservation activities from cache. Cached for one request.
        /// </summary>
        public static IList<ActivityModel> Activities(this CacheManager cm)
        {
            IList<ActivityModel> result = cm.GetContextItem<IList<ActivityModel>>("Activities");

            if (result == null || result.Count == 0)
            {
                result = cm.ServiceProvider.DataAccess.Session.Query<Activity>().Model<ActivityModel>();
                cm.SetContextItem("Activities", result);
            }

            return result;
        }

        /// <summary>
        /// Gets the reservation activities specified by filter from cache. Cached for one request.
        /// </summary>
        public static IList<ActivityModel> Activities(this CacheManager cm, Func<ActivityModel, bool> filter)
        {
            return cm.Activities().Where(filter).ToList();
        }

        public static ActivityModel GetActivity(this CacheManager cm, int activityId)
        {
            var result = cm.Activities(x => x.ActivityID == activityId).FirstOrDefault();

            if (result == null)
                throw new CacheItemNotFoundException<Activity>(x => x.ActivityID, activityId);

            return result;
        }

        public static IList<ActivityModel> AuthorizedActivities(this CacheManager cm, ClientAuthLevel authLevel)
        {
            //procActivitySelect @Action = 'SelectAuthorizedActivities'

            //SELECT ActivityID, ActivityName, ListOrder, Chargeable, Editable, AccountType, UserAuth, InviteeType, InviteeAuth,
            //    StartEndAuth, NoReservFenceAuth, NoMaxSchedAuth, [Description], IsActive, IsFacilityDownTime
            //FROM Activity
            //WHERE UserAuth &@UserAuth > 0
            //    AND Editable = 1 AND IsActive = 1
            //    AND IsFacilityDownTime = 0--This keeps the Facility Down Time activity out of the select when making reservations. Staff should use the link at the top of the page instead.
            //ORDER BY ListOrder

            IList<ActivityModel> result = cm.Activities().Where(x => x.IsActive && (x.UserAuth & (int)authLevel) > 0 && x.Editable && !x.IsFacilityDownTime).OrderBy(x => x.ListOrder).ToList();

            return result;
        }

        public static ClientSetting GetClientSetting(this CacheManager cm)
        {
            return cm.GetSessionValue(SessionKeys.ClientSetting, () => ClientSetting.GetClientSettingOrDefault(cm.ClientID));
        }

        public static IList<ResourceClientModel> CurrentResourceClients(this CacheManager cm)
        {
            IList<ResourceClientModel> result = cm.GetSessionValue(SessionKeys.CurrentResourceClients, () =>
            {
                int clientId = cm.ClientID;
                var query = cm.ServiceProvider.DataAccess.Session.Query<ResourceClientInfo>().Where(x => x.ClientID == clientId || x.ClientID == -1);
                var models = query.Model<ResourceClientModel>();
                return models;
            });

            return result;
        }

        public static ResourceClientModel GetCurrentResourceClient(this CacheManager cm, int resourceId)
        {
            return cm.CurrentResourceClients().FirstOrDefault(x => x.ResourceID == resourceId);
        }

        public static IList<ResourceClientModel> ResourceClients(this CacheManager cm, int resourceId)
        {
            string key = "ResourceClients#" + resourceId;

            var result = cm.GetContextItem<IList<ResourceClientModel>>(key);

            if (result == null || result.Count > 0)
            {
                result = cm.ServiceProvider.DataAccess.Session.Query<ResourceClientInfo>().Where(x => x.ResourceID == resourceId).Model<ResourceClientModel>();
                cm.SetContextItem(key, result);
            }

            return result;
        }

        public static IList<ResourceClientModel> ToolEngineers(this CacheManager cm, int resourceId)
        {
            return cm.ResourceClients(resourceId).Where(x => (x.AuthLevel & ClientAuthLevel.ToolEngineer) > 0).ToList();
        }

        public static ResourceClientModel GetResourceClient(this CacheManager cm, int resourceId, int clientId)
        {
            return cm.ResourceClients(resourceId).FirstOrDefault(x => x.ClientID == clientId || x.ClientID == -1);
        }

        public static ClientAuthLevel GetAuthLevel(this CacheManager cm, int resourceId, int clientId)
        {
            var client = cm.GetClient(clientId);
            var resourceClients = cm.ResourceClients(resourceId);
            return DA.Current.ReservationManager().GetAuthLevel(resourceClients, client, resourceId);
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

        public static void WeekStartDate(this CacheManager cm, DateTime value)
        {
            cm.SetSessionValue(SessionKeys.WeekStartDate, value);
        }

        public static bool IsOnKiosk(this CacheManager cm)
        {
            return cm.GetSessionValue(SessionKeys.IsOnKiosk, () =>
            {
                string kioskIp = cm.ServiceProvider.Context.UserHostAddress;
                return KioskUtility.IsOnKiosk(cm.GetClientLabs(), kioskIp);
            });
        }

        public static bool ClientInLab(this CacheManager cm, int labId)
        {
            return KioskUtility.ClientInLab(cm.GetClientLabs(), labId);
        }

        public static int[] GetClientLabs(this CacheManager cm)
        {
            return cm.GetSessionValue(SessionKeys.ClientLabs, () =>
            {
                string kioskIp = cm.ServiceProvider.Context.UserHostAddress;
                return KioskUtility.IpCheck(cm.ClientID, kioskIp);
            });
        }

        public static bool DisplayDefaultHours(this CacheManager cm)
        {
            return cm.GetSessionValue(SessionKeys.DisplayDefaultHours, () => true);
        }

        public static void DisplayDefaultHours(this CacheManager cm, bool value)
        {
            cm.SetSessionValue(SessionKeys.DisplayDefaultHours, value);
        }

        public static IList<ProcessInfoModel> ProcessInfos(this CacheManager cm, int resourceId)
        {
            string key = "ProcessInfos#" + resourceId.ToString();

            IList<ProcessInfoModel> result = cm.GetContextItem<IList<ProcessInfoModel>>(key);

            if (result == null || result.Count == 0)
            {
                result = cm.ServiceProvider.DataAccess.Session.Query<ProcessInfo>().Where(x => x.Resource.ResourceID == resourceId).Model<ProcessInfoModel>();
                cm.SetContextItem(key, result);
            }

            return result;
        }

        public static IList<ProcessInfoLineModel> ProcessInfoLines(this CacheManager cm, int processInfoId)
        {
            string key = "ProcessInfoLines#" + processInfoId.ToString();

            IList<ProcessInfoLineModel> result = cm.GetContextItem<IList<ProcessInfoLineModel>>(key);

            if (result == null || result.Count == 0)
            {
                result = cm.ServiceProvider.DataAccess.Session.Query<ProcessInfoLine>().Where(x => x.ProcessInfoID == processInfoId).Model<ProcessInfoLineModel>();
                cm.SetContextItem(key, result);
            }

            return result;
        }

        public static IList<ReservationProcessInfoItem> ReservationProcessInfos(this CacheManager cm)
        {
            return cm.GetSessionValue(SessionKeys.ReservationProcessInfos, () => new List<ReservationProcessInfoItem>());
        }

        public static void ReservationProcessInfos(this CacheManager cm, IList<ReservationProcessInfoItem> value)
        {
            cm.SetSessionValue(SessionKeys.ReservationProcessInfos, value);
        }

        public static IList<ReservationInviteeItem> ReservationInvitees(this CacheManager cm)
        {
            return cm.GetSessionValue<IList<ReservationInviteeItem>>(SessionKeys.ReservationInvitees, () => null);
        }

        public static void ReservationInvitees(this CacheManager cm, IList<ReservationInviteeItem> value)
        {
            cm.SetSessionValue(SessionKeys.ReservationInvitees, value);
        }

        public static IList<ReservationInviteeItem> RemovedInvitees(this CacheManager cm)
        {
            return cm.GetSessionValue<IList<ReservationInviteeItem>>(SessionKeys.RemovedInvitees, () => null);
        }

        public static void RemovedInvitees(this CacheManager cm, IList<ReservationInviteeItem> value)
        {
            cm.SetSessionValue(SessionKeys.RemovedInvitees, value);
        }

        public static IList<AvailableInviteeItem> AvailableInvitees(this CacheManager cm)
        {
            return cm.GetSessionValue<IList<AvailableInviteeItem>>(SessionKeys.AvailableInvitees, () => null);
        }

        public static void AvailableInvitees(this CacheManager cm, IList<AvailableInviteeItem> value)
        {
            cm.SetSessionValue(SessionKeys.AvailableInvitees, value);
        }

        public static ViewType CurrentViewType(this CacheManager cm)
        {
            return cm.GetSessionValue("CurrentViewType", () => ViewType.WeekView);
        }

        public static void CurrentViewType(this CacheManager cm, ViewType value)
        {
            cm.SetSessionValue("CurrentViewType", value);
        }

        public static ClientAuthLevel SelectAuthLevel(this CacheManager cm, ResourceModel item, IPrivileged client)
        {
            return cm.GetAuthLevel(item.ResourceID, client.ClientID);
        }
    }

    public static class SessionExtensions
    {
        public static ResourceManager ResourceManager(this ISession session)
        {
            return new ResourceManager(session);
        }

        public static ReservationManager ReservationManager(this ISession session)
        {
            return new ReservationManager(session);
        }

        public static ReservationInviteeManager ReservationInviteeManager(this ISession session)
        {
            return new ReservationInviteeManager(session);
        }

        public static EmailManager EmailManager(this ISession session)
        {
            return new EmailManager(session);
        }
    }
}
