using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF;
using LNF.Repository;
using LNF.Repository.Scheduler;

namespace LNF.Scheduler
{
    public class ActivityUtility
    {
        public static IList<Activity> ActiveActivities()
        {
            return DA.Current.Query<Activity>()
                .Where(x => x.IsActive)
                .OrderBy(x => x.ActivityName)
                .ToList();
        }
    }
}
