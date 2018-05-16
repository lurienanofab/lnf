using LNF.Repository;
using LNF.Repository.Scheduler;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Scheduler.Data
{
    /// <summary>
    /// A class for handling AuthLevel data.
    /// </summary>
    public static class AuthLevelData
    {
        /// <summary>
        /// Returns all auth levels 
        /// </summary>
        public static IEnumerable<AuthLevel> SelectAll()
        {
            // Replaces sselScheduler.dbo.procAuthLevelSelect @Action = 'SelectAll'
            return DA.Current.Query<AuthLevel>().OrderBy(x => x.AuthLevelID).ToList();
        }

        public static IEnumerable<AuthLevel> SelectAuthorizable()
        {
            // Replaces sselScheduler.dbo.procAuthLevelSelect @Action = 'SelectAuthorizable'
            return DA.Current.Query<AuthLevel>().Where(x => x.Authorizable == 1).OrderBy(x => x.AuthLevelID).ToList();
        }
    }
}
