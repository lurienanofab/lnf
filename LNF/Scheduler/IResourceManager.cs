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
        IQueryable<ReservationRecurrence> GetReservationRecurrences(ResourceItem item);
        Resource GetResource(ResourceItem item);
        IQueryable<ResourceActivityAuth> GetResourceActivityAuths(ResourceItem item);
        IQueryable<ResourceClientInfo> GetResourceClients(ResourceItem item);
        IEnumerable<ResourceCost> GetResourceCosts(DateTime? cutoff = null);
        IEnumerable<ResourceCost> GetResourceCosts(ResourceItem item, DateTime? cutoff = null);
        IQueryable<ResourceClientInfo> GetToolEngineers(ResourceItem item);
        IQueryable<ResourceClientInfo> SelectNotifyOnCancelClients(ResourceItem item);
        IQueryable<ResourceClientInfo> SelectNotifyOnOpeningClients(ResourceItem item);
        IQueryable<ResourceClientInfo> SelectNotifyOnPracticeRes(ResourceItem item);
    }
}