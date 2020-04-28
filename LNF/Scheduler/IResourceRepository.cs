using System;
using System.Collections.Generic;

namespace LNF.Scheduler
{
    public interface IResourceRepository
    {
        IResource GetResource(int resourceId);
        IEnumerable<IResource> Select();
        IEnumerable<IResource> SelectActive();
        IEnumerable<IResource> SelectByLab(int? labId);
        string GetInterlockStatus(int resourceId);
        IEnumerable<IResourceActivityAuth> GetResourceActivityAuths(int resourceId);
        IEnumerable<IResourceClient> GetResourceClients(int resourceId = 0, int clientId = 0, ClientAuthLevel authLevel = 0);
        IEnumerable<IProcessInfoLine> GetProcessInfoLines(int resourceId);
        IEnumerable<IResourceClient> GetActiveResourceClients(int resourceId = 0, int clientId = 0, ClientAuthLevel authLevel = 0);
        IEnumerable<IResourceClient> GetExpiringResourceClients(bool everyone = false);
        IEnumerable<IResourceClient> GetExpiredResourceClients(bool everyone = false);
        int DeleteExpiredResourceClients();
        IEnumerable<IOnTheFlyResource> GetOnTheFlyResources();
        IOnTheFlyResource GetOnTheFlyResource(int resourceId);
        IEnumerable<IResourceCost> GetResourceCosts(DateTime? cutoff = null);
        IEnumerable<IResourceCost> GetResourceCosts(int resourceId, DateTime? cutoff = null);
        IEnumerable<IResourceStatus> GetResourceStatuses(int[] tools);
        IEnumerable<IAuthLevel> GetAuthLevels();
        IEnumerable<GenericListItem> AllActiveResources();
        int[] GetOffsets(int granularity);
        IEnumerable<ReservationTime> GetMinReservationTime(int granularity);
        int[] GetMaxReservationTime(int granularity, int minReservTime);
        int[] GetGracePeriodHour(int granularity, int minReservTime);
        int[] GetGracePeriodMinute(int granularity, int minReservTime, int gracePeriodHour);
        IEnumerable<IResource> GetResources(IEnumerable<int> ids);
        IEnumerable<IResourceTree> GetResourceTree(int clientId);
        ILab GetLab(int labId);
        IEnumerable<ILab> GetLabs();
        void UpdateResourceState(int resourceId, ResourceState state, string stateNotes);
        void UpdateResource(int resourceId, string resourceName, decimal useCost, decimal hourlyCost, int authDuration, bool authState, int reservFence, int granularity, int offset, int minReservTime, int maxReservTime, int maxAlloc, int minCancelTime, int gracePeriod, int autoEnd, int? otfSchedTime, string ipAddress, string description, string wikiPageUrl, bool isReady, int? unloadTime);
        int UpdateExpiration(int resourceClientId, DateTime expiration);
        IBuilding GetBuilding(int buildingId);
    }
}
