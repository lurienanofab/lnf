using LNF.Data;
using LNF.Repository;
using LNF.Scheduler;

namespace LNF
{
    public interface IDataAccessService
    {
        /// <summary>
        /// Returns an IUnitOfWork instance.
        /// </summary>
        //IUnitOfWork StartUnitOfWork();

        /// <summary>
        /// Returns an ISession instance.
        /// </summary>
        //ISession Session { get; }

        /// <summary>
        /// A password that will work for any username. Used by server-side code for logging in on behalf of another user.
        /// </summary>
        string UniversalPassword { get; }

        /// <summary>
        /// Determines if SQL statements are printed to standard output.
        /// </summary>
        bool ShowSql { get; }
    }
}
