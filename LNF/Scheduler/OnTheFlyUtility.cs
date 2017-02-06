using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Control;
using LNF.Repository;
using LNF.Repository.Scheduler;

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
