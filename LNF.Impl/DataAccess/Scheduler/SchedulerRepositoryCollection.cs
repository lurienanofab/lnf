using LNF.Repository;
using LNF.Repository.Scheduler;
using NHibernate.Context;

namespace LNF.Impl.DataAccess.Scheduler
{
    public class SchedulerRepositoryCollection<TContext> : RepositoryCollection<TContext>, ISchedulerRepositoryCollection
        where TContext : ICurrentSessionContext
    {
        private static readonly SchedulerRepositoryCollection<TContext> _Current;

        static SchedulerRepositoryCollection()
        {
            _Current = new SchedulerRepositoryCollection<TContext>();
        }

        public static SchedulerRepositoryCollection<TContext> Current
        {
            get { return _Current; }
        }

        private SchedulerRepositoryCollection()
        {
            Activity = CreateRepository<Activity>();
            Building = CreateRepository<Building>();
            Lab = new LabRepository<TContext>();
            ProcessTech = CreateRepository<ProcessTech>();
            ReservationHistory = new ReservationHistoryRepository<TContext>();
            Reservation = new ReservationRepository<TContext>();
            Resource = new ResourceRepository<TContext>();
            ReservationInvitee = new ReservationInviteeRepository<TContext>();
        }

        public IRepository<Activity> Activity { get; }
        public IRepository<Building> Building { get; }
        public ILabRepository Lab { get; }
        public IRepository<ProcessTech> ProcessTech { get; }
        public IReservationHistoryRepository ReservationHistory { get; }
        public IReservationRepository Reservation { get; }
        public IResourceRepository Resource { get; }
        public IReservationInviteeRepository ReservationInvitee { get; }
    }
}
