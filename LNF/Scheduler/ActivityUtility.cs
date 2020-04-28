using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public class ActivityUtility
    {
        public static IList<IActivity> ActiveActivities()
        {
            return ServiceProvider.Current.Scheduler.Activity.GetActiveActivities().OrderBy(x => x.ActivityName).ToList();
        }
    }
}
