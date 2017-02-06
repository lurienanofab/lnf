using LNF.CommonTools;
using LNF.Repository;
using LNF.Repository.Scheduler;
using NHibernate.Context;
using System.Data;
using System.Linq;

namespace LNF.Impl.DataAccess.Scheduler
{
    public class ReservationInviteeRepository<TContext> : Repository<TContext, ReservationInvitee>, IReservationInviteeRepository
        where TContext : ICurrentSessionContext
    {
        private IRepository<Activity> Activity
        {
            get { return SchedulerRepositoryCollection<TContext>.Current.Activity; }
        }

        public int[] SelectAvailable(int reservationId, int resourceId, int activityId, int clientId)
        {
            // reservationId == -1 means a new reservation.

            // clientId belongs to the current user so that they are not listed as an available client.

            using (var dba = DA.Current.GetAdapter())
            {
                var dt = dba
                    .ApplyParameters(new { Action = "SelectAvailInvitees", ReservationID = reservationId, ResourceID = resourceId, ActivityID = activityId, InviteeID = clientId })
                    .FillDataTable("sselScheduler.dbo.procReservationInviteeSelect");

                return dt.AsEnumerable().Select(x => x.Field<int>("ClientID")).ToArray();
            }
        }
    }
}
