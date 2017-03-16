using LNF.Repository;
using LNF.Repository.Scheduler;
using System.Linq;

namespace LNF.Scheduler
{
    public static class OnTheFlyUtility
    {
        public static uint GetStateDuration(int resourceId)
        {
            OnTheFlyResource r = DA.Current.Query<OnTheFlyResource>().FirstOrDefault(x => x.Resource.ResourceID == resourceId);

            if (r != null && r.ResourceType == OnTheFlyResourceType.Cabinet)
                return r.ResourceStateDuration > 0 ? (uint)r.ResourceStateDuration : 0;

            return 0;
        }
    }
}
