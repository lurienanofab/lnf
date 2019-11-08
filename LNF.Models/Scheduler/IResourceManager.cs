using System;
using System.Collections.Generic;

namespace LNF.Models.Scheduler
{
    public interface IResourceManager
    {
        IResource GetResource(int resourceId);
        IEnumerable<IResource> Select();
        IEnumerable<IResource> SelectActive();
        IEnumerable<IResource> SelectByLab(int? labId);
        string GetInterlockStatus(int resourceId);
        IEnumerable<IResourceActivityAuth> GetResourceActivityAuths(int resourceId);
        IEnumerable<IResourceClient> GetResourceClients(int resourceId);
        IEnumerable<IResourceCost> GetResourceCosts(DateTime? cutoff = null);
        IEnumerable<IResourceCost> GetResourceCosts(int resourceId, DateTime? cutoff = null);
        IEnumerable<IResourceClient> GetToolEngineers(int resourceId);
        IEnumerable<IResourceClient> SelectNotifyOnCancelClients(int resourceId);
        IEnumerable<IResourceClient> SelectNotifyOnOpeningClients(int resourceId);
        IEnumerable<IResourceClient> SelectNotifyOnPracticeRes(int resourceId);
        IEnumerable<ListItem> AllActiveResources();
        int[] GetOffsets(int granularity);
        IEnumerable<ReservationTime> GetMinReservationTime(int granularity);
        int[] GetMaxReservationTime(int granularity, int minReservTime);
        int[] GetGracePeriodHour(int granularity, int minReservTime);
        int[] GetGracePeriodMinute(int granularity, int minReservTime, int gracePeriodHour);
        IEnumerable<IResource> GetResources(IEnumerable<int> ids);
    }
}
