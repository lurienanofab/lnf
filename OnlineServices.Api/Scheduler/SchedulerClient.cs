using LNF.Models;
using LNF.Models.Scheduler;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Scheduler
{
    public class SchedulerClient : ApiClient, ISchedulerService
    {
        public SchedulerClient() : base(GetApiBaseUrl()) { }

        public IEnumerable<ReservationItem> GetReservations(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0, int activityId = 0, bool? started = null, bool? active = null)
        {
            var qs = GetQueryStringParametersForReservations(sd, ed, clientId, resourceId, activityId, started, active);
            return Get<List<ReservationItem>>("webapi/scheduler/reservation", qs);
        }

        public IEnumerable<ReservationItemWithInvitees> GetReservationsWithInvitees(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0, int activityId = 0, bool? started = null, bool? active = null)
        {
            var qs = GetQueryStringParametersForReservations(sd, ed, clientId, resourceId, activityId, started, active);
            return Get<List<ReservationItemWithInvitees>>("webapi/scheduler/reservation/with-invitees", qs);
        }

        public ReservationItem GetReservation(int reservationId)
        {
            return Get<ReservationItem>("webapi/scheduler/reservation/{reservationId}", UrlSegments(new { reservationId }));
        }

        public ReservationItemWithInvitees GetReservationWithInvitees(int reservationId)
        {
            return Get<ReservationItemWithInvitees>("webapi/scheduler/reservation/{reservationId}/with-invitees", UrlSegments(new { reservationId }));
        }

        public IEnumerable<ReservationInviteeItem> GetReservationInvitees(int reservationId)
        {
            return Get<List<ReservationInviteeItem>>("webapi/scheduler/reservation/{reservationId}/invitees", UrlSegments(new { reservationId }));
        }

        public bool UpdateReservationHistory(ReservationHistoryUpdate model)
        {
            return Put("webapi/scheduler/reservation/history", model);
        }

        public IEnumerable<ReservationStateItem> GetReservationStates(DateTime sd, DateTime ed, string kioskIp, int? clientId = null, int? resourceId = null, int? reserverId = null)
        {
            var qs = new ParameterCollection
            {
                { "sd", sd.ToString("yyyy-MM-dd"), ParameterType.QueryString },
                { "ed", ed.ToString("yyyy-MM-dd"), ParameterType.QueryString },
                { "kioskIp", kioskIp, ParameterType.QueryString }
            };

            if (clientId.HasValue)
                qs.Add("clientId", clientId.Value, ParameterType.QueryString);

            if (resourceId.HasValue)
                qs.Add("resourceId", resourceId.Value, ParameterType.QueryString);

            if (reserverId.HasValue)
                qs.Add("reserverId", reserverId.Value, ParameterType.QueryString);

            return Get<List<ReservationStateItem>>("webapi/scheduler/reservation/states", qs);
        }

        public ResourceItem GetResource(int resourceId)
        {
            return Get<ResourceItem>("webapi/scheduler/resource/{resourceId}", UrlSegments(new { resourceId }));
        }

        private ParameterCollection GetQueryStringParametersForReservations(DateTime sd, DateTime ed, int clientId, int resourceId, int activityId, bool? started, bool? active)
        {
            var result = new ParameterCollection
            {
                { "sd", sd.ToString("yyyy-MM-dd"), ParameterType.QueryString },
                { "ed", ed.ToString("yyyy-MM-dd"), ParameterType.QueryString },
                { "clientId", clientId, ParameterType.QueryString },
                { "resourceId", resourceId, ParameterType.QueryString },
                { "activityId", activityId, ParameterType.QueryString }
            };

            if (started.HasValue)
                result.Add("started", started.Value, ParameterType.QueryString);

            if (active.HasValue)
                result.Add("active", active.Value, ParameterType.QueryString);

            return result;
        }
    }
}
