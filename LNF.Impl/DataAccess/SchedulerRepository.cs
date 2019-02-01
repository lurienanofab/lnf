using LNF.Repository.Scheduler;
using LNF.Scheduler;
using NHibernate;
using NHibernate.Criterion;
using System.Collections.Generic;

namespace LNF.Impl.DataAccess.Scheduler
{
    public class SchedulerRepository : ISchedulerRepository
    {
        private readonly ISessionManager _sessionManager;

        protected ISession Session => _sessionManager.Session;
        
        public SchedulerRepository(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public IEnumerable<Reservation> SelectRecentReservations(int resourceId, int? take = null)
        {
            // procReservationSelect @Action = 'SelectRecent'

            //SELECT TOP 6 Rv.ReservationID, Rv.BeginDateTime, Rv.EndDateTime,
            //  Rv.ClientID, sselData.dbo.udf_GetDisplayName(RV.ClientID) AS DisplayName
            //FROM dbo.Reservation Rv
            //WHERE Rv.ResourceID = @ResourceID
            //ORDER BY ABS (datediff (second, Rv.BeginDateTime, getdate())) ASC

            // Need to use criterion because of the complicated order by clause.

            var result = Session.CreateCriteria<Reservation>()
                .Add(Restrictions.Eq("Resource.ResourceID", resourceId))
                .AddOrder(Order.Asc(Projections.SqlFunction("abs", NHibernateUtil.Int32, DateProjections.DateDiff(DatePart.Second, Projections.Property<Reservation>(x => x.BeginDateTime), DateProjections.GetDate()))))
                .SetMaxResults(take.GetValueOrDefault(6))
                .List<Reservation>();

            return result;
        }
    }
}
