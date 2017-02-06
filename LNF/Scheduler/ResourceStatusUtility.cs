using LNF.Repository;
using LNF.Repository.Scheduler;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public static class ResourceStatusUtility
    {
        public static IList<ResourceStatus> SelectByToolList(int[] tools)
        {
            IList<ResourceStatus> result = DA.Current.Query<ResourceStatus>().Where(x => tools.Contains(x.ResourceID)).ToList();
            return result;
        }
    }
}
