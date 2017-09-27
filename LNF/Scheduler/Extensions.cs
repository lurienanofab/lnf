using LNF.Cache;
using LNF.CommonTools;
using LNF.Data;
using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LNF.Scheduler
{
    public static class CacheManagerExtensions
    {
        public static IList<ResourceTree> ResourceTree(this CacheManager cm)
        {
            // always for the current user

            IList<ResourceTree> result = cm.GetContextItem<IList<ResourceTree>>("ResourceTree");

            if (result == null || result.Count == 0)
            {
                result = DA.Current.Query<ResourceTree>().Where(x => x.ClientID == cm.ClientID).ToList();
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
                result = DA.Current.Query<Activity>().Model<ActivityModel>();
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
                throw new CacheItemNotFoundException("Activity", "ActivityID", activityId);

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

        /// <summary>
        /// Gets all buildings from cache. Cached for one request.
        /// </summary>
        public static IList<BuildingModel> Buildings(this CacheManager cm)
        {
            IList<BuildingModel> result = cm.GetContextItem<IList<BuildingModel>>("Buildings");

            if (result == null || result.Count == 0)
            {
                result = cm.ResourceTree().GetBuildings();
                cm.SetContextItem("Buildings", result);
            }

            return result;
        }

        /// <summary>
        /// Gets the buildings specified by filter from cache. Cached for one request.
        /// </summary>
        public static IList<BuildingModel> Buildings(this CacheManager cm, Func<BuildingModel, bool> filter)
        {
            var result = cm.Buildings().Where(filter).ToList();
            return result;
        }

        public static BuildingModel GetBuilding(this CacheManager cm, int buildingId)
        {
            var result = cm.Buildings(x => x.BuildingID == buildingId).FirstOrDefault();

            if (result == null)
                throw new CacheItemNotFoundException("Building", "BuildingID", buildingId);

            return result;
        }

        /// <summary>
        /// Gets all labs from cache. Cached for one request.
        /// </summary>
        public static IList<LabModel> Labs(this CacheManager cm)
        {
            IList<LabModel> result = cm.GetContextItem<IList<LabModel>>("Labs");

            if (result == null || result.Count == 0)
            {
                result = cm.ResourceTree().GetLabs();
                cm.SetContextItem("Labs", result);
            }

            return result;
        }

        /// <summary>
        /// Gets the labs specified by filter from cache. Cached for one request.
        /// </summary>
        public static IList<LabModel> Labs(this CacheManager cm, Func<LabModel, bool> filter)
        {
            var result = cm.Labs().Where(filter).ToList();
            return result;
        }

        public static LabModel GetLab(this CacheManager cm, int labId)
        {
            var result = cm.Labs(x => x.LabID == labId).FirstOrDefault();

            if (result == null)
                throw new CacheItemNotFoundException("Lab", "LabID", labId);

            return result;
        }

        /// <summary>
        /// Gets all process techs from cache. Cached for one request.
        /// </summary>
        public static IList<ProcessTechModel> ProcessTechs(this CacheManager cm)
        {
            IList<ProcessTechModel> result = cm.GetContextItem<IList<ProcessTechModel>>("ProcessTechs");

            if (result == null || result.Count == 0)
            {
                result = cm.ResourceTree().GetProcessTechs();
                cm.SetContextItem("ProcessTechs", result);
            }

            return result;
        }

        /// <summary>
        /// Gets process techs specified by filter from cache. Cached for one request.
        /// </summary>
        public static IList<ProcessTechModel> ProcessTechs(this CacheManager cm, Func<ProcessTechModel, bool> filter)
        {
            var result = cm.ProcessTechs().Where(filter).ToList();
            return result;
        }

        public static ProcessTechModel GetProcessTech(this CacheManager cm, int processTechId)
        {
            var result = cm.ProcessTechs(x => x.ProcessTechID == processTechId).FirstOrDefault();

            if (result == null)
                throw new CacheItemNotFoundException("ProcessTech", "ProcessTechID", processTechId);

            return result;
        }

        /// <summary>
        /// Gets all resources from cache. Cached for one request.
        /// </summary>
        public static IList<ResourceModel> Resources(this CacheManager cm)
        {
            IList<ResourceModel> result = cm.GetContextItem<IList<ResourceModel>>("Resources");

            if (result == null || result.Count == 0)
            {
                result = cm.ResourceTree().GetResources();
                cm.SetContextItem("Resources", result);
            }

            return result;
        }

        /// <summary>
        /// Gets the resources specified by filter from cache. Cached for one request.
        /// </summary>
        public static IList<ResourceModel> Resources(this CacheManager cm, Func<ResourceModel, bool> filter)
        {
            return cm.Resources().Where(filter).ToList();
        }

        public static ResourceModel GetResource(this CacheManager cm, int resourceId)
        {
            var result = cm.Resources(x => x.ResourceID == resourceId).FirstOrDefault();

            if (result == null)
                throw new CacheItemNotFoundException("Resource", "ResourceID", resourceId);

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
                var query = DA.Current.Query<ResourceClientInfo>().Where(x => x.ClientID == clientId || x.ClientID == -1);
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
                result = DA.Current.Query<ResourceClientInfo>().Where(x => x.ResourceID == resourceId).Model<ResourceClientModel>();
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
            ClientModel client = cm.GetClient(clientId);

            if (client.HasPriv(ClientPrivilege.Administrator | ClientPrivilege.Developer))
                return ClientAuthLevel.ToolEngineer;

            var rc = cm.GetResourceClient(resourceId, clientId);

            if (rc == null)
                return ClientAuthLevel.UnauthorizedUser;

            if (resourceId == 0)
                return ClientAuthLevel.UnauthorizedUser;

            return rc.AuthLevel;
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

        public static IList<ResourceCostModel> ToolCosts(this CacheManager cm, DateTime cutoff, int resourceId)
        {
            string key = "ResourceCosts#" + resourceId.ToString();

            IList<ResourceCostModel> result = cm.GetContextItem<IList<ResourceCostModel>>(key);

            if (result == null || result.Count == 0)
            {
                result = CostUtility.FindCosts("ToolCost", cutoff, null, resourceId).Model<ResourceCostModel>();
                cm.SetContextItem(key, result);
            }

            return result;
        }

        public static IList<ResourceCostModel> ToolCosts(this CacheManager cm, DateTime cutoff, int resourceId, int chargeTypeId)
        {
            return cm.ToolCosts(cutoff, resourceId).Where(x => x.ChargeTypeID == chargeTypeId).ToList();
        }

        public static bool IsOnKiosk(this CacheManager cm)
        {
            return cm.GetSessionValue(SessionKeys.IsOnKiosk, () =>
            {
                string kioskIp = Providers.Context.Current.UserHostAddress;
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
                string kioskIp = Providers.Context.Current.UserHostAddress;
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
                result = DA.Current.Query<ProcessInfo>().Where(x => x.Resource.ResourceID == resourceId).Model<ProcessInfoModel>();
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
                result = DA.Current.Query<ProcessInfoLine>().Where(x => x.ProcessInfoID == processInfoId).Model<ProcessInfoLineModel>();
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
    }

    public static class ResourceModelExtensions
    {
        public static Resource GetResource(this ResourceModel item)
        {
            var result = DA.Scheduler.Resource.Single(item.ResourceID);

            if (result == null)
                throw new InvalidOperationException(string.Format("No Resource found for ResourceID = {0}", item.ResourceID));

            return result;
        }

        public static IQueryable<ResourceClientInfo> GetResourceClients(this ResourceModel item)
        {
            return ResourceClientInfoUtility.GetResourceClients(item.ResourceID);
        }

        public static IQueryable<ResourceClientInfo> GetToolEngineers(this ResourceModel item)
        {
            return ResourceClientInfoUtility.GetToolEngineers(item.ResourceID);
        }

        public static IQueryable<ResourceClientInfo> SelectNotifyOnCancelClients(this ResourceModel item)
        {
            return ResourceClientInfoUtility.SelectNotifyOnCancelClients(item.ResourceID);
        }

        public static IQueryable<ResourceClientInfo> SelectNotifyOnOpeningClients(this ResourceModel item)
        {
            return ResourceClientInfoUtility.SelectNotifyOnOpeningClients(item.ResourceID);
        }

        public static IQueryable<ResourceClientInfo> SelectNotifyOnPracticeRes(this ResourceModel item)
        {
            return ResourceClientInfoUtility.SelectNotifyOnPracticeRes(item.ResourceID);
        }

        public static IQueryable<ReservationRecurrence> GetReservationRecurrences(this ResourceModel item)
        {
            return DA.Current.Query<ReservationRecurrence>().Where(x => x.Resource.ResourceID == item.ResourceID);
        }

        public static IQueryable<ResourceActivityAuth> GetResourceActivityAuths(this ResourceModel item)
        {
            return DA.Current.Query<ResourceActivityAuth>().Where(x => x.Resource.ResourceID == item.ResourceID);
        }

        public static IList<ResourceCost> GetResourceCosts(this ResourceModel item)
        {
            IEnumerable<Cost> costs = DA.Current.Query<Cost>().Where(x => (x.RecordID == item.ResourceID && x.TableNameOrDescription == "ToolCost") || x.TableNameOrDescription == "ToolOvertimeCost");
            return ResourceCost.GetAll(costs, item.ResourceID);
        }

        public static double SelectReservableMinutes(this ResourceModel item, int clientId, DateTime now)
        {
            return ReservationUtility.SelectReservableMinutes(item.ResourceID, clientId, item.ReservFence, item.MaxAlloc, now);
        }

        public static async Task<string> GetInterlockStatus(this ResourceModel item)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ResourceID", typeof(int));
            dt.Columns.Add("InterlockStatus", typeof(string));
            dt.Rows.Add(item.ResourceID, string.Empty);
            await WagoInterlock.AllToolStatus(dt);
            DataRow dr = dt.Select(string.Format("ResourceID = {0}", item.ResourceID)).FirstOrDefault();
            string result = dr["InterlockStatus"].ToString();
            return result;
        }

        public static DateTime? OpenResSlot(this ResourceModel item, DateTime now, DateTime sd)
        {
            return ReservationUtility.OpenResSlot(item.ResourceID, item.ReservFence, item.MinReservTime, now, sd);
        }

        public static ClientAuthLevel SelectAuthLevel(this ResourceModel item, IPrivileged client)
        {
            return CacheManager.Current.GetAuthLevel(item.ResourceID, client.ClientID);
        }

        /// <summary>
        /// Returns the next grain boundary in the past or future
        /// </summary>
        /// <param name="actualTime">The point in time to determine the next or previous granularity</param>
        /// <param name="granDir">The direction (next or pervious) to search in</param>
        /// <returns>The DateTime value of the next or previous granularity</returns>
        public static DateTime GetNextGranularity(this ResourceModel item, DateTime actualTime, NextGranDir granDir)
        {
            return ResourceUtility.GetNextGranularity(item.Granularity, item.Offset, actualTime, granDir);
        }

        /// <summary>
        /// Sets the start and end time slot boundaries
        /// </summary>
        /// <param name="startTime">The start time</param>
        /// <param name="endTime">The end time</param>
        public static void GetTimeSlotBoundary(this ResourceModel item, ref DateTime startTime, ref DateTime endTime)
        {
            ResourceUtility.GetTimeSlotBoundary(item.Granularity, item.Offset, ref startTime, ref endTime);
        }
    }

    public static class ResourceTreeExtensions
    {
        public static IList<BuildingModel> GetBuildings(this IEnumerable<ResourceTree> tree)
        {
            var distinct = tree.Select(x => new { x.BuildingID, x.BuildingName, x.BuildingDescription, x.BuildingIsActive }).Distinct();

            var result = distinct.OrderBy(x => x.BuildingID).Select(x => new BuildingModel()
            {
                BuildingID = x.BuildingID,
                BuildingName = x.BuildingName,
                BuildingDescription = x.BuildingDescription,
                BuildingIsActive = x.BuildingIsActive
            }).ToList();

            return result;
        }

        public static IList<LabModel> GetLabs(this IEnumerable<ResourceTree> tree)
        {
            var distinct = tree.Select(x => new { x.LabID, x.LabName, x.LabDisplayName, x.LabDescription, x.LabIsActive, x.RoomID, x.RoomName, x.BuildingID, x.BuildingName, x.BuildingIsActive }).Distinct();

            var result = distinct.OrderBy(x => x.LabID).Select(x => new LabModel()
            {
                LabID = x.LabID,
                LabName = x.LabName,
                LabDisplayName = x.LabDisplayName,
                LabDescription = x.LabDescription,
                LabIsActive = x.LabIsActive,
                RoomID = x.RoomID,
                RoomName = x.RoomName,
                BuildingID = x.BuildingID,
                BuildingName = x.BuildingName,
                BuildingIsActive = x.BuildingIsActive
            }).ToList();

            return result;
        }

        public static IList<ProcessTechModel> GetProcessTechs(this IEnumerable<ResourceTree> tree)
        {
            var distinct = tree.Select(x => new { x.ProcessTechID, x.ProcessTechName, x.ProcessTechDescription, x.ProcessTechIsActive, x.ProcessTechGroupID, x.ProcessTechGroupName, x.LabID, x.LabName, x.LabDisplayName, x.LabDescription, x.LabIsActive, x.RoomID, x.RoomName, x.BuildingID, x.BuildingName, x.BuildingIsActive }).Distinct();

            var result = distinct.OrderBy(x => x.LabID).Select(x => new ProcessTechModel()
            {
                ProcessTechID = x.ProcessTechID,
                ProcessTechName = x.ProcessTechName,
                ProcessTechDescription = x.ProcessTechDescription,
                ProcessTechIsActive = x.ProcessTechIsActive,
                GroupID = x.ProcessTechGroupID,
                GroupName = x.ProcessTechGroupName,
                LabID = x.LabID,
                LabName = x.LabName,
                LabDisplayName = x.LabDisplayName,
                LabIsActive = x.LabIsActive,
                BuildingID = x.BuildingID,
                BuildingName = x.BuildingName,
                BuildingIsActive = x.BuildingIsActive
            }).ToList();

            return result;
        }

        public static IList<ResourceModel> GetResources(this IEnumerable<ResourceTree> tree)
        {
            var distinct = tree.Distinct();

            var result = distinct.OrderBy(x => x.LabID).Select(x => new ResourceModel()
            {
                AuthDuration = x.AuthDuration,
                AuthState = x.AuthState,
                AutoEnd = TimeSpan.FromMinutes(x.AutoEnd),
                GracePeriod = TimeSpan.FromMinutes(x.GracePeriod),
                Granularity = TimeSpan.FromMinutes(x.Granularity),
                HelpdeskEmail = x.HelpdeskEmail,
                IsReady = x.IsReady,
                IsSchedulable = x.IsSchedulable,
                MaxAlloc = TimeSpan.FromMinutes(x.MaxAlloc),
                MaxReservTime = TimeSpan.FromMinutes(x.MaxReservTime),
                MinCancelTime = TimeSpan.FromMinutes(x.MinCancelTime),
                MinReservTime = TimeSpan.FromMinutes(x.MinReservTime),
                Offset = TimeSpan.FromHours(x.Offset),
                ReservFence = TimeSpan.FromMinutes(x.ReservFence),
                ResourceID = x.ResourceID,
                ResourceIsActive = x.ResourceIsActive,
                ResourceName = x.ResourceName,
                ResourceDescription = x.ResourceDescription,
                State = x.State,
                StateNotes = x.StateNotes,
                UnloadTime = TimeSpan.FromMinutes(x.UnloadTime),
                WikiPageUrl = x.WikiPageUrl,
                ProcessTechID = x.ProcessTechID,
                ProcessTechName = x.ProcessTechName,
                LabID = x.LabID,
                LabName = x.LabName,
                LabDisplayName = x.LabDisplayName,
                BuildingID = x.BuildingID,
                BuildingName = x.BuildingName
            }).ToList();

            return result;
        }

        public static ActivityModel GetCurrentActivity(this IEnumerable<ResourceTree> tree, int resourceId)
        {
            ResourceTree item = tree.Where(x => x.ResourceID == resourceId).FirstOrDefault();

            if (item == null) return null;

            if (item.CurrentActivityID == 0) return null;

            var result = CacheManager.Current.GetActivity(item.CurrentActivityID);

            return result;
        }

        public static ClientModel GetCurrentClient(this IEnumerable<ResourceTree> tree, int resourceId)
        {
            ResourceTree item = tree.Where(x => x.ResourceID == resourceId).FirstOrDefault();

            if (item == null) return null;

            if (item.CurrentClientID == 0) return null;

            var result = CacheManager.Current.GetClient(item.CurrentClientID);

            return result;
        }

        public static ClientModel GetClient(this IEnumerable<ResourceTree> tree)
        {
            var item = tree.Select(x => new { x.ClientID }).FirstOrDefault();

            if (item == null) return null;

            if (item.ClientID == 0) return null;

            var result = CacheManager.Current.GetClient(item.ClientID);

            return result;
        }
    }

    public static class DataTableExtensions
    {
        public static IList<ReservationInvitee> ToReservationInviteeList(this DataTable dt, int reservationId)
        {
            if (dt == null) return null;

            if (dt.Columns.Contains("ReservationID") && dt.Columns.Contains("InviteeID"))
            {
                var result = new List<ReservationInvitee>();
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        int rsvId = RepositoryUtility.ConvertTo(dr["ReservationID"], 0);

                        if (rsvId <= 0) rsvId = reservationId;

                        int inviteeId = RepositoryUtility.ConvertTo(dr["InviteeID"], 0);

                        if (rsvId != 0 && inviteeId != 0)
                        {
                            var reservation = DA.Current.Single<Reservation>(rsvId);
                            var invitee = DA.Current.Single<Client>(inviteeId);

                            var ri = ReservationInviteeUtility.Select(reservation, invitee);

                            if (ri != null)
                                result.Add(ri);
                        }
                    }
                }
                return result;
            }
            else
                throw new Exception("ReservationID and InviteeID columns are required.");
        }
    }
}
