using System.Collections.Generic;

namespace LNF.Scheduler
{
    public static class ResourceStatuses
    {
        public static IEnumerable<IResourceStatus> SelectByToolList(int[] tools)
        {
            return ServiceProvider.Current.Scheduler.Resource.GetResourceStatuses(tools);
        }
    }
}
