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
        public static IEnumerable<IAuthLevel> SelectAll()
        {
            // Replaces sselScheduler.dbo.procAuthLevelSelect @Action = 'SelectAll'
            return ServiceProvider.Current.Scheduler.Resource
                .GetAuthLevels().OrderBy(x => x.AuthLevelID).ToList();
        }

        public static IEnumerable<IAuthLevel> SelectAuthorizable()
        {
            // Replaces sselScheduler.dbo.procAuthLevelSelect @Action = 'SelectAuthorizable'
            return ServiceProvider.Current.Scheduler.Resource
                .GetAuthLevels().Where(X => X.Authorizable == 1).OrderBy(x => x.AuthLevelID).ToList();
        }
    }
}
