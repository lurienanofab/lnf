using LNF.CommonTools;
using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Mail;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Scheduler;
using LNF.Scheduler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace LNF.Impl.Scheduler
{
    public class ResourceRepository : RepositoryBase, IResourceRepository
    {
        protected ICostRepository Cost { get; }

        public ResourceRepository(ISessionManager mgr, ICostRepository cost) : base(mgr)
        {
            Cost = cost;
        }

        public IResource GetResource(int resourceId)
        {
            return Session.Get<ResourceInfo>(resourceId);
        }

        public IEnumerable<IResource> GetResources(IEnumerable<int> ids)
        {
            return Session.Query<ResourceInfo>().Where(x => ids.Contains(x.ResourceID));
        }

        public IEnumerable<IResource> Select()
        {
            return Session.Query<ResourceInfo>()
                .OrderBy(x => x.BuildingName)
                .ThenBy(x => x.LabName)
                .ThenBy(x => x.ProcessTechName)
                .ThenBy(x => x.ResourceName);
        }

        public IEnumerable<IResource> SelectActive()
        {
            return Session.Query<ResourceInfo>()
                .Where(x => x.ResourceIsActive)
                .OrderBy(x => x.BuildingName)
                .ThenBy(x => x.LabName)
                .ThenBy(x => x.ProcessTechName)
                .ThenBy(x => x.ResourceName);
        }

        public IEnumerable<IResource> SelectByLab(int? labId)
        {
            // when labId is null use "default labs"
            // when labId is 0 use "all labs"
            // when labId > 0 select for single lab

            //procResourceSelect @Action='SelectByLabID'

            /*if @LabID is null
                Select ResourceName, ResourceID
                from Resource
                where IsActive = 1
                and LabID in (1, 9)	
			    order by ResourceName
		    else
			    Select ResourceName, ResourceID
                from Resource
                where IsActive = 1
                and LabID = case when @LabID = 0 then LabID else @LabID end
                order by ResourceName*/

            int[] defaultLabs = { 1, 9 };

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DefaultLabs"]))
                defaultLabs = ConfigurationManager.AppSettings["DefaultLabs"].Split(',').Select(int.Parse).ToArray();

            IQueryable<ResourceInfo> query;

            if (labId == null)
                query = Session.Query<ResourceInfo>().Where(x => x.ResourceIsActive && defaultLabs.Contains(x.LabID));
            else
            {
                if (labId.Value == 0)
                    query = Session.Query<ResourceInfo>().Where(x => x.ResourceIsActive);
                else
                    query = Session.Query<ResourceInfo>().Where(x => x.ResourceIsActive && x.LabID == labId.Value);
            }

            var result = query.OrderBy(x => x.BuildingName).ThenBy(x => x.LabDisplayName).ThenBy(x => x.ProcessTechName).ThenBy(x => x.ResourceName);

            return result;
        }

        public IEnumerable<IResourceTree> GetResourceTree(int clientId)
        {
            var query = Session.GetNamedQuery("SelectResourceTree").SetParameter("ClientID", clientId);
            var result = query.List<ResourceTree>();
            return result;
        }

        public IEnumerable<IResourceActivityAuth> GetResourceActivityAuths(int resourceId)
        {
            return Session.Query<ResourceActivityAuth>()
                .Where(x => x.Resource.ResourceID == resourceId)
                .CreateModels<IResourceActivityAuth>();
        }

        public IEnumerable<IResourceClient> GetResourceClients(int resourceId = 0, int clientId = 0, ClientAuthLevel authLevel = 0)
        {
            return Session.SelectResourceClients(resourceId, clientId, authLevel).ToList();
        }

        public IEnumerable<IResourceClient> GetActiveResourceClients(int resourceId = 0, int clientId = 0, ClientAuthLevel authLevel = 0)
        {
            return Session.SelectResourceClients(resourceId, clientId, authLevel)
                .Where(x => x.Expiration == null || x.Expiration.Value > DateTime.Now)
                .ToList();
        }

        public IEnumerable<IResourceClient> GetExpiringResourceClients(bool everyone = false)
        {
            var authExpWarning = double.Parse(Session.Query<SchedulerProperty>().First(x => x.PropertyName == "AuthExpWarning").PropertyValue);

            var query = Session.Query<ResourceClientInfo>()
                .Where(x => x.AuthLevel == ClientAuthLevel.AuthorizedUser && x.Expiration != null);

            if (everyone)
                query = query.Where(x => x.ClientID == -1);
            else
                query = query.Where(x => x.ClientID != -1);

            var result = query.ToList().Where(x => DateTime.Now > x.WarningDate(authExpWarning)).ToList();

            return result;
        }

        public IEnumerable<IResourceClient> GetExpiredResourceClients(bool everyone = false)
        {
            var query = Session.Query<ResourceClientInfo>()
                .Where(x => x.AuthLevel == ClientAuthLevel.AuthorizedUser && x.Expiration != null && x.Expiration.Value < DateTime.Now && x.ResourceIsActive);

            if (everyone)
                query = query.Where(x => x.ClientID == -1);
            else
                query = query.Where(x => x.ClientID != -1);

            var result = query.ToList();

            return result;
        }

        public int DeleteExpiredResourceClients()
        {
            var expiredClients = Session.Query<ResourceClient>().Where(x => x.AuthLevel <= ClientAuthLevel.AuthorizedUser && x.Expiration != null && x.Expiration.Value < DateTime.Now).ToList();

            var result = expiredClients.Count;

            foreach (var rc in expiredClients)
                Session.Delete(rc);

            return result;
        }

        public IEnumerable<IOnTheFlyResource> GetOnTheFlyResources()
        {
            return Session.Query<OnTheFlyResource>().ToList();
        }

        public IOnTheFlyResource GetOnTheFlyResource(int resourceId)
        {
            return Session.Query<OnTheFlyResource>().FirstOrDefault(x => x.ResourceID == resourceId);
        }

        public IEnumerable<IResourceCost> GetResourceCosts(DateTime? cutoff = null)
        {
            var costs = Cost.FindToolCosts(cutoff);
            return ResourceCost.CreateResourceCosts(costs);
        }

        public IEnumerable<IResourceCost> GetResourceCosts(int resourceId, DateTime? cutoff = null)
        {
            var costs = Cost.FindToolCosts(resourceId, cutoff);
            return ResourceCost.CreateResourceCosts(costs);
        }

        public IEnumerable<IResourceStatus> GetResourceStatuses(int[] tools)
        {
            return Session.Query<ResourceStatus>().Where(x => tools.Contains(x.ResourceID)).ToList();
        }

        public string GetInterlockStatus(int resourceId)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ResourceID", typeof(int));
            dt.Columns.Add("InterlockStatus", typeof(string));
            dt.Rows.Add(resourceId, string.Empty);
            WagoInterlock.AllToolStatus(dt);
            DataRow dr = dt.Select($"ResourceID = {resourceId}").FirstOrDefault();

            string result;

            if (dr != null)
                result = Convert.ToString(dr["InterlockStatus"]);
            else
                result = $"{resourceId} not found";

            return result;
        }

        public IEnumerable<GenericListItem> AllActiveResources()
        {
            var groupEmailManager = new GroupEmailManager(Session);
            var dv = groupEmailManager.GetAllActiveTools();

            var result = new List<GenericListItem>();

            foreach (DataRowView drv in dv)
            {
                result.Add(new GenericListItem(Convert.ToString(drv["ResourceID"]), Convert.ToString(drv["ResourceName"])));
            }

            return result;
        }

        public int[] GetOffsets(int granularity)
        {
            var result = new List<int> { 0 };

            if (granularity > 60)
                result.Add(1);

            if (granularity > 120)
                result.Add(2);

            return result.ToArray();
        }

        public IEnumerable<ReservationTime> GetMinReservationTime(int granularity)
        {
            // Load Hours
            var result = new List<ReservationTime>();

            for (int i = 1; i <= 6; i++)
            {
                double minReservTime = i * granularity;
                TimeSpan ts = TimeSpan.FromMinutes(minReservTime);
                double day, hour, minute;

                //hour = Math.Floor(minReservTime / 60);
                //minute = minReservTime % 60;
                day = ts.Days;
                hour = ts.Hours;
                minute = ts.Minutes;

                string text = string.Empty;

                if (day > 0) text += string.Format("{0} day ", day);
                if (hour > 0) text += string.Format("{0} hr ", hour);
                if (minute > 0) text += string.Format("{0} min ", minute);

                result.Add(ReservationTime.Create(minReservTime, text.Trim()));
            }

            return result;
        }

        public int[] GetMaxReservationTime(int granularity, int minReservTime)
        {
            //                                                                  1               2   3    6   12   24      days
            int[] maxReservTimeList = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 18, 24, 30, 36, 42, 48, 72, 144, 288, 576 }; //hours

            // the max is 576 because the max granularity is now 1440 (1440 * 24 / 60 = 576)

            int maxValue = Convert.ToInt32(granularity * 24 / 60);
            int minValue = Convert.ToInt32(Math.Ceiling((double)minReservTime / 60));

            var result = new List<int>();

            for (int i = 0; i < maxReservTimeList.Length; i++)
            {
                int h = maxReservTimeList[i];
                if (h > maxValue) break;
                if (h >= minValue && (h * 60) % granularity == 0)
                    result.Add(h);
            }

            return result.ToArray();
        }

        public int[] GetGracePeriodHour(int granularity, int minReservTime)
        {
            var maxHour = Convert.ToInt32(Math.Floor((double)minReservTime / 60));

            var result = new List<int>();

            var stepSize = Convert.ToInt32(Math.Ceiling((double)granularity / 60));

            int minValue = 0;

            if (granularity >= 60) minValue = stepSize;

            for (int i = minValue; i <= maxHour; i += stepSize)
            {
                result.Add(i);
            }

            return result.ToArray();
        }

        public int[] GetGracePeriodMinute(int granularity, int minReservTime, int gracePeriodHour)
        {
            var maxHour = Convert.ToInt32(Math.Floor((double)minReservTime / 60));

            var result = new List<int>();

            if (gracePeriodHour == maxHour && granularity < 60)
            {
                var maxMinute = minReservTime % 60;
                for (int i = 0; i <= maxMinute; i += granularity)
                {
                    result.Add(i);
                }
            }
            else
            {
                var count = Convert.ToInt32(Math.Ceiling(60 / (double)granularity));
                for (int i = 0; i < count; i++)
                {
                    var minute = granularity * i;
                    result.Add(minute);
                }
            }

            return result.ToArray();
        }

        public IBuilding GetBuilding(int buildingId)
        {
            return Session.Get<Building>(buildingId).CreateModel<IBuilding>();
        }

        public ILab GetLab(int labId)
        {
            return Session.Get<Lab>(labId).CreateModel<ILab>();
        }

        public IEnumerable<ILab> GetLabs()
        {
            return Session.Query<Lab>().CreateModels<ILab>();
        }

        public IEnumerable<IAuthLevel> GetAuthLevels()
        {
            return Session.Query<AuthLevel>().ToList();
        }

        public void UpdateResourceState(int resourceId, ResourceState state, string stateNotes)
        {
            // procResourceUpdate @Action = 'UpdateState'

            //UPDATE dbo.Resource
            //SET State = @State,
            //StateNotes = @StateNotes
            //WHERE ResourceID = @ResourceID

            var res = Require<Resource>(resourceId);

            res.State = state;
            res.StateNotes = stateNotes;

            Session.Update(res);
        }

        public void UpdateResource(int resourceId, string resourceName, decimal useCost, decimal hourlyCost, int authDuration, bool authState, int reservFence, int granularity, int offset, int minReservTime, int maxReservTime, int maxAlloc, int minCancelTime, int gracePeriod, int autoEnd, int? otfSchedTime, string ipAddress, string description, string wikiPageUrl, bool isReady, int? unloadTime)
        {
            // procResourceUpdate @Action = 'EngineerUpdate'

            //UPDATE dbo.Resource
            //SET ResourceName = @ResourceName,
            //  UseCost = @UseCost,
            //  HourlyCost = @HourlyCost,
            //  AuthDuration = @AuthDuration,
            //  AuthState = @AuthState,
            //  ReservFence = @ReservFence * 60,
            //  Granularity = @Granularity,
            //  Offset = @Offset,
            //  MinReservTime = @MinReservTime,
            //  MaxReservTime = @MaxReservTime * 60,
            //  MaxAlloc = @MaxAlloc * 60,
            //  MinCancelTime = @MinCancelTime,
            //  GracePeriod = @GracePeriod,
            //  AutoEnd = @AutoEnd,
            //  OTFSchedTime = @OTFSchedTime,
            //  IPAddress = @IPAddress,
            //  Description = @Description,
            //  IsReady = 1,
            //  UnloadTime = @UnloadTime
            //WHERE ResourceID = @ResourceID

            // need to check ReservFence, MaxReservTime, and MaxAlloc

            var res = Require<Resource>(resourceId);

            res.ResourceName = resourceName;
            res.UseCost = useCost;
            res.HourlyCost = hourlyCost;
            res.AuthDuration = authDuration;
            res.AuthState = authState;
            res.ReservFence = reservFence;
            res.Granularity = granularity;
            res.Offset = offset;
            res.MinReservTime = minReservTime;
            res.MaxReservTime = maxReservTime;
            res.MaxAlloc = maxAlloc;
            res.MinCancelTime = minCancelTime;
            res.GracePeriod = gracePeriod;
            res.AutoEnd = autoEnd;
            res.OTFSchedTime = otfSchedTime;
            res.IPAddress = ipAddress;
            res.Description = description;
            res.WikiPageUrl = wikiPageUrl;
            res.IsReady = isReady;
            res.UnloadTime = unloadTime;

            Session.Update(res);
        }

        public int UpdateExpiration(int resourceClientId, DateTime expiration)
        {
            return Session
                .CreateSQLQuery("EXEC sselScheduler.dbo.procResourceClientUpdate @Action = 'UpdateExpiration', @ResourceClientID = :ResourceClientID, @Expiration = :Expiration")
                .SetParameter("ResourceClientID", resourceClientId)
                .SetParameter("Expiration", expiration)
                .ExecuteUpdate();
        }

        public IEnumerable<IProcessInfoLine> GetProcessInfoLines(int resourceId)
        {
            var query = Session.Query<ProcessInfo>().Where(x => x.Resource.ResourceID == resourceId);

            var result = query.Join(Session.Query<ProcessInfoLine>(),
                o => o.ProcessInfoID,
                i => i.ProcessInfoID,
                (o, i) => new ProcessInfoLineItem
                {
                    ProcessInfoLineID = i.ProcessInfoLineID,
                    ProcessInfoID = o.ProcessInfoID,
                    Param = i.Param,
                    MinValue = i.MinValue,
                    MaxValue = i.MaxValue,
                    ProcessInfoLineParamID = i.ProcessInfoLineParam.ProcessInfoLineParamID,
                    ResourceID = o.Resource.ResourceID,
                    ResourceName = o.Resource.ResourceName,
                    ParameterName = i.ProcessInfoLineParam.ParameterName,
                    ParameterType = i.ProcessInfoLineParam.ParameterType
                }).ToList();

            return result;
        }
    }
}
