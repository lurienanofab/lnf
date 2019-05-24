using LNF.Models.Scheduler;
using System;
using System.Collections.Generic;

namespace LNF.Impl.Scheduler
{
    public class SchedulerService : ISchedulerService
    {
        public ReservationItem GetReservation(int reservationId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ReservationInviteeItem> GetReservationInvitees(int reservationId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ReservationItem> GetReservations(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0, int activityId = 0, bool? started = null, bool? active = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ReservationStateItem> GetReservationStates(DateTime sd, DateTime ed, string kioskIp, int? clientId = null, int? resourceId = null, int? reserverId = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ReservationItemWithInvitees> GetReservationsWithInvitees(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0, int activityId = 0, bool? started = null, bool? active = null)
        {
            throw new NotImplementedException();
        }

        public ReservationItemWithInvitees GetReservationWithInvitees(int reservationId)
        {
            throw new NotImplementedException();
        }

        public ResourceItem GetResource(int resourceId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateReservationHistory(ReservationHistoryUpdate model)
        {
            throw new NotImplementedException();
        }
    }
}
