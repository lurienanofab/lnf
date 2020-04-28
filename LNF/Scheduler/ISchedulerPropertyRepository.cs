using System.Collections.Generic;

namespace LNF.Scheduler
{
    public interface ISchedulerPropertyRepository
    {
        IEnumerable<ISchedulerProperty> GetSchedulerProperties();
    }
}