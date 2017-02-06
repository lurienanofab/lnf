using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using LNF.Repository.Scheduler;

namespace LNF.Repository
{
    public static class DA
    {
        private const string UOW_KEY = "CurrentUnitOfWork";

        /// <summary>
        /// Gets an IRepository instance that uses the current data access context.
        /// </summary>
        public static IRepository Current
        {
            get { return Providers.DataAccess.Repository; }
        }

        public static ISchedulerRepositoryCollection Scheduler
        {
            get { return Providers.DataAccess.Scheduler; }
        }

        //The methods that were previously defined here have been moved to LNF.UnitOfWork
        //I think these methods should only be defined once, so now they are accessed via Current
    }
}
