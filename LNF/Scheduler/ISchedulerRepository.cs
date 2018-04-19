using LNF.Repository.Scheduler;
using System.Collections.Generic;

namespace LNF.Scheduler
{
    /// <summary>
    /// This interface is for requirements that cannot be met by LNF.Repository.ISession data interactions.
    /// </summary>
    public interface ISchedulerRepository
    {
        IEnumerable<Reservation> SelectRecentReservations(int resourceId, int? take = null);
    }
}
