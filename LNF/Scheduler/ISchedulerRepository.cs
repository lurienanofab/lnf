using LNF.Models.Scheduler;
using System;
using System.Collections.Generic;

namespace LNF.Scheduler
{
    /// <summary>
    /// This interface is for requirements that cannot be met by LNF.Repository.ISession data interactions.
    /// </summary>
    public interface ISchedulerRepository
    {
        IEnumerable<ResourceCostModel> GetToolCosts(DateTime cutoff, int resourceId);
        IEnumerable<ResourceCostModel> GetToolCosts(DateTime cutoff, int resourceId, int chargeTypeId);
    }
}
