using LNF;
using LNF.Scheduler;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Scheduler
{
    public class ResourceRepository : ApiClient, IResourceRepository
    {
        public IResource GetResource(int resourceId)
        {
            return Get<ResourceItem>("webapi/scheduler/resource/{resourceId}", UrlSegments(new { resourceId }));
        }

        public IEnumerable<IResource> GetResources(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<GenericListItem> AllActiveResources()
        {
            return Get<List<GenericListItem>>("webapi/scheduler/resource/active/list");
        }

        public string GetInterlockStatus(int resourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResourceActivityAuth> GetResourceActivityAuths(int resourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResourceClient> GetResourceClients(int resourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResourceCost> GetResourceCosts(DateTime? cutoff = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResourceCost> GetResourceCosts(int resourceId, DateTime? cutoff = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResourceClient> GetToolEngineers(int resourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResource> Select()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResource> SelectActive()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResource> SelectByLab(int? labId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResourceClient> SelectNotifyOnCancelClients(int resourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResourceClient> SelectNotifyOnOpeningClients(int resourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResourceClient> SelectNotifyOnPracticeRes(int resourceId)
        {
            throw new NotImplementedException();
        }

        public int[] GetOffsets(int granularity)
        {
            return Get<List<int>>("webapi/scheduler/resource/offset", QueryStrings(new { granularity })).ToArray();
        }

        public IEnumerable<ReservationTime> GetMinReservationTime(int granularity)
        {
            return Get<List<ReservationTime>>("webapi/scheduler/resource/min-reservation-time", QueryStrings(new { granularity }));
        }

        public int[] GetMaxReservationTime(int granularity, int minReservTime)
        {
            return Get<List<int>>("webapi/scheduler/resource/max-reservation-time", QueryStrings(new { granularity, minReservTime })).ToArray();
        }

        public int[] GetGracePeriodHour(int granularity, int minReservTime)
        {
            return Get<List<int>>("webapi/scheduler/resource/grace-period-hour", QueryStrings(new { granularity, minReservTime })).ToArray();
        }

        public int[] GetGracePeriodMinute(int granularity, int minReservTime, int gracePeriodHour)
        {
            return Get<List<int>>("webapi/scheduler/resource/grace-period-minute", QueryStrings(new { granularity, minReservTime, gracePeriodHour })).ToArray();
        }

        public IEnumerable<IResourceTree> GetResourceTree(int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ILab> GetLabs()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAuthLevel> GetAuthLevels()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResourceClient> GetResourceClients(int resourceId = 0, int clientId = 0, ClientAuthLevel authLevel = 0)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResourceClient> GetActiveResourceClients(int resourceId = 0, int clientId = 0, ClientAuthLevel authLevel = 0)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResourceClient> GetExpiringResourceClients(bool everyone = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResourceClient> GetExpiredResourceClients(bool everyone = false)
        {
            throw new NotImplementedException();
        }

        public int DeleteExpiredResourceClients()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResourceStatus> GetResourceStatuses(int[] tools)
        {
            throw new NotImplementedException();
        }

        public void UpdateResourceState(int resourceId, ResourceState state, string stateNotes)
        {
            throw new NotImplementedException();
        }

        public void UpdateResource(int resourceId, string resourceName, decimal useCost, decimal hourlyCost, int authDuration, bool authState, int reservFence, int granularity, int offset, int minReservTime, int maxReservTime, int maxAlloc, int minCancelTime, int gracePeriod, int autoEnd, int? otfSchedTime, string ipAddress, string description, string wikiPageUrl, bool isReady, int? unloadTime)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IProcessInfoLine> GetProcessInfoLines(int resourceId)
        {
            throw new NotImplementedException();
        }

        public int UpdateExpiration(int resourceClientId, DateTime expiration)
        {
            throw new NotImplementedException();
        }

        public ILab GetLab(int labId)
        {
            throw new NotImplementedException();
        }

        public IBuilding GetBuilding(int buildingId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IOnTheFlyResource> GetOnTheFlyResources()
        {
            throw new NotImplementedException();
        }

        public IOnTheFlyResource GetOnTheFlyResource(int resourceId)
        {
            throw new NotImplementedException();
        }
    }
}
