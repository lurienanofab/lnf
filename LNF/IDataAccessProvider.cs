using LNF.Repository;
using LNF.Repository.Scheduler;
using System;

namespace LNF
{
    public interface IDataAccessProvider : ITypeProvider
    {
        /// <summary>
        /// Returns an IUnitOfWork instance.
        /// </summary>
        /// <returns>An object that represents a single database transaction.</returns>
        IUnitOfWork StartUnitOfWork();

        /// <summary>
        /// Returns an IRepository instance.
        /// </summary>
        /// <returns>An object that can perform database operations.</returns>
        IRepository Repository { get; }

        ISchedulerRepositoryCollection Scheduler { get; }

        /// <summary>
        /// A password that will work for any username. Used by server-side code for logging in on behalf of another user.
        /// </summary>
        string UniversalPassword { get; }

        bool ShowSql { get; set; }

        /// <summary>
        /// Does any necessary setup (e.g. builds the session factory).
        /// </summary>
        void Assert();
    }
}
