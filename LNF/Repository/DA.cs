using LNF.Data;
using LNF.Scheduler;

namespace LNF.Repository
{
    public static class DA
    {
        private const string UOW_KEY = "CurrentUnitOfWork";

        /// <summary>
        /// Gets an ISession instance that uses the current data access context.
        /// </summary>
        public static ISession Current => ServiceProvider.Current.Resolver.GetInstance<ISession>();

        public static T Use<T>() where T : IManager => ServiceProvider.Current.Resolver.GetInstance<T>();

        public static ISchedulerRepository SchedulerRepository => ServiceProvider.Current.Resolver.GetInstance<ISchedulerRepository>();

        //The methods that were previously defined here have been moved to LNF.Repository.ISession
        //I think these methods should only be defined once, so now they are accessed via Current
    }
}
