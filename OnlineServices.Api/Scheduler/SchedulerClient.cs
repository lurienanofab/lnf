using LNF.Models.Scheduler;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineServices.Api.Scheduler
{
    public class SchedulerClient : ApiClient
    {
        internal SchedulerClient(ApiClientOptions options) : base(options) { }

        public async Task<ReservationModel> GetReservation(int reservationId)
        {
            return await Get<ReservationModel>(string.Format("scheduler/reservation/{0}", reservationId));
        }

        public async Task<IEnumerable<ReservationModel>> GetReservations(DateTime sd, DateTime ed, int clientId = 0, int activityId = 0, bool? started = null, bool? active = null)
        {
            string qs = string.Empty;
            
            if (started.HasValue)
                qs += "&started=" + started.Value.ToString().ToLower();

            if (active.HasValue)
                qs += "&active=" + active.Value.ToString().ToLower();

            return await Get<IEnumerable<ReservationModel>>(string.Format("scheduler/reservation?sd={0:yyyy-MM-dd}&ed={1:yyyy-MM-dd}&clientId={2}&activityId={3}{4}", sd, ed, clientId, activityId, qs));
        }

        public async Task<bool> UpdateHistory(ReservationHistoryUpdate model)
        {
            return await Put<bool>("scheduler/reservation/history", model);
        }
    }
}
