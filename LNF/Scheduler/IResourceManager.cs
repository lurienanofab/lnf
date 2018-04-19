using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LNF.Scheduler
{
    public interface IResourceManager : IManager
    {
        IQueryable<Resource> SelectActive();
        IQueryable<Resource> SelectByLab(int? labId);
        Task<string> GetInterlockStatus(ResourceModel item);
        DateTime GetNextGranularity(ResourceModel item, DateTime actualTime, NextGranDir granDir);
        IQueryable<ReservationRecurrence> GetReservationRecurrences(ResourceModel item);
        Resource GetResource(ResourceModel item);
        IQueryable<ResourceActivityAuth> GetResourceActivityAuths(ResourceModel item);
        IQueryable<ResourceClientInfo> GetResourceClients(ResourceModel item);
        IList<ResourceCost> GetResourceCosts(ResourceModel item);
        void GetTimeSlotBoundary(ResourceModel item, ref DateTime startTime, ref DateTime endTime);
        IQueryable<ResourceClientInfo> GetToolEngineers(ResourceModel item);
        IQueryable<ResourceClientInfo> SelectNotifyOnCancelClients(ResourceModel item);
        IQueryable<ResourceClientInfo> SelectNotifyOnOpeningClients(ResourceModel item);
        IQueryable<ResourceClientInfo> SelectNotifyOnPracticeRes(ResourceModel item);
        IEnumerable<ResourceCostModel> GetToolCosts(DateTime cutoff, int resourceId);
        IEnumerable<ResourceCostModel> GetToolCosts(DateTime cutoff, int resourceId, int chargeTypeId);
    }
}