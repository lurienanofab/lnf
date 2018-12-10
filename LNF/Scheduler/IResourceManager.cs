using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public interface IResourceManager : IManager
    {
        IQueryable<Resource> SelectActive();
        IQueryable<Resource> SelectByLab(int? labId);
        string GetInterlockStatus(ResourceItem item);
        DateTime GetNextGranularity(ResourceItem item, DateTime actualTime, GranularityDirection granDir);
        IQueryable<ReservationRecurrence> GetReservationRecurrences(ResourceItem item);
        Resource GetResource(ResourceItem item);
        IQueryable<ResourceActivityAuth> GetResourceActivityAuths(ResourceItem item);
        IQueryable<ResourceClientInfo> GetResourceClients(ResourceItem item);
        IEnumerable<ResourceCost> GetResourceCosts(DateTime? cutoff = null);
        IEnumerable<ResourceCost> GetResourceCosts(ResourceItem item, DateTime? cutoff = null);
        void GetTimeSlotBoundary(ResourceItem item, ref DateTime startTime, ref DateTime endTime);
        IQueryable<ResourceClientInfo> GetToolEngineers(ResourceItem item);
        IQueryable<ResourceClientInfo> SelectNotifyOnCancelClients(ResourceItem item);
        IQueryable<ResourceClientInfo> SelectNotifyOnOpeningClients(ResourceItem item);
        IQueryable<ResourceClientInfo> SelectNotifyOnPracticeRes(ResourceItem item);
    }
}