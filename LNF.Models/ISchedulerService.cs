using LNF.Models.Scheduler;
using System;
using System.Collections.Generic;

namespace LNF.Models
{
    public interface ISchedulerService
    {
        IEnumerable<ReservationItem> GetReservations(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0, int activityId = 0, bool? started = null, bool? active = null);
        IEnumerable<ReservationWithInvitees> GetReservationsWithInvitees(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0, int activityId = 0, bool? started = null, bool? active = null);
        ReservationItem GetReservation(int reservationId);
        ReservationWithInvitees GetReservationWithInvitees(int reservationId);
        bool UpdateReservationHistory(ReservationHistoryUpdate model);
        IEnumerable<ReservationInviteeItem> GetReservationInvitees(int reservationId);
        IEnumerable<ReservationStateItem> GetReservationStates(DateTime sd, DateTime ed, string kioskIp, int? clientId = null, int? resourceId = null, int? reserverId = null);

        ResourceItem GetResource(int resourceId);
    }
}
