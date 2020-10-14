using LNF.Data;
using LNF.Scheduler;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;

namespace OnlineServices.Api.Scheduler
{
    public class ReservationRepository : ApiClient, IReservationRepository
    {
        public void AppendNotes(int reservationId, string notes)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientAccount> AvailableAccounts(IReservation item)
        {
            throw new NotImplementedException();
        }

        public void CanCreateCheck(IReservation item, DateTime now, int? modifiedByclientId)
        {
            throw new NotImplementedException();
        }

        public IReservation CreateReservation(int resourceId, int clientId, int accountId, int activityId, DateTime beginDateTime, DateTime endDateTime, double duration, string notes, bool autoEnd, bool hasProcessInfo, bool hasInvitees, int? recurrenceId, bool isActive, bool keepAlive, double maxReservedDuration, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public void DeleteAndForgive(IReservation item, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public int DeleteByGroup(int groupId, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public int DeleteByRecurrence(int recurrenceId, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public void DeleteReservation(IReservation item, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public void EndForRepair(IReservation item, int? endedByClientId, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public int EndPastUnstarted(IReservation item, DateTime endDate, int? endedByClientId)
        {
            throw new NotImplementedException();
        }

        public void EndReservation(EndReservationArgs args)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ReservationHistoryFilterItem> FilterCancelledReservations(IEnumerable<IReservation> reservations, bool includeCanceledForModification)
        {
            throw new NotImplementedException();
        }

        public int GetAvailableSchedMin(int resourceId, int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservation> GetCurrentReservations()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservationHistory> GetHistory(int reservationId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservationInvitee> GetInvitees(int reservationId)
        {
            return Get<List<ReservationInvitee>>("webapi/scheduler/reservation/{reservationId}/invitees", UrlSegments(new { reservationId }));
        }

        public IReservation GetReservation(int reservationId)
        {
            return Get<ReservationItem>("webapi/scheduler/reservation/{reservationId}", UrlSegments(new { reservationId }));
        }

        public IEnumerable<IReservation> GetReservations(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0, int activityId = 0, bool? started = null, bool? active = null)
        {
            var qs = GetQueryStringParametersForReservations(sd, ed, clientId, resourceId, activityId, started, active);
            return Get<List<ReservationItem>>("webapi/scheduler/reservation", qs);
        }

        public IReservationWithInvitees GetReservationWithInvitees(int reservationId)
        {
            return Get<ReservationWithInviteesItem>("webapi/scheduler/reservation/{reservationId}/with-invitees", UrlSegments(new { reservationId }));
        }

        public IEnumerable<IReservationWithInvitees> GetReservationsWithInvitees(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0, int activityId = 0, bool? started = null, bool? active = null)
        {
            var qs = GetQueryStringParametersForReservations(sd, ed, clientId, resourceId, activityId, started, active);
            return Get<List<ReservationWithInviteesItem>>("webapi/scheduler/reservation/with-invitees", qs);
        }

        public IEnumerable<IReservationRecurrence> GetReservationRecurrencesByResource(int resourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservationRecurrence> GetReservationRecurrencesByProcessTech(int processTechId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservationRecurrence> GetReservationRecurrencesByLabLocation(int labLocationId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservationRecurrence> GetReservationRecurrencesByClient(int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResourceClient> GetResourceClients(int resourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResource> GetResources(IEnumerable<IReservation> reservations)
        {
            throw new NotImplementedException();
        }

        public TimeSpan GetTimeUntilNextReservation(IResource res, int reservationId, int clientId, DateTime beginDateTime)
        {
            throw new NotImplementedException();
        }

        public void InsertFacilityDownTime(IReservation item, DateTime now, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public IReservation InsertForModification(InsertReservationArgs args)
        {
            throw new NotImplementedException();
        }

        public IReservation InsertRepair(int resourceId, int clientId, DateTime beginDateTime, DateTime endDateTime, DateTime actualBeginDateTime, string notes, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public void InsertReservation(IReservation item, DateTime now, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public bool IsInvited(int reservationId, int clientId)
        {
            throw new NotImplementedException();
        }

        public int PurgeReservation(int reservationId)
        {
            throw new NotImplementedException();
        }

        public int PurgeReservations(int resourceId, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservation> ReservationsInGranularityWindow(IResource res)
        {
            throw new NotImplementedException();
        }

        public SaveReservationHistoryResult SaveReservationHistory(IReservation item, int accountId, double? forgivenPct, string notes, bool emailClient)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservation> SelectAutoEnd()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservation> SelectByClient(int clientId, DateTime sd, DateTime ed, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservation> SelectByDateRange(DateTime sd, DateTime ed, int clientId = 0)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservation> SelectByGroup(int groupId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservation> SelectByProcessTech(int processTechId, DateTime sd, DateTime ed, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservation> SelectByLabLocation(int labLocationId, DateTime sd, DateTime ed, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservation> SelectByResource(int resourceId, DateTime sd, DateTime ed, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservation> SelectEndableReservations(int resourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservation> SelectExisting(int resourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservation> SelectHistory(int clientId, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservation> SelectHistoryToForgiveForRepair(int resourceId, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public DateTime? SelectLastRepairEndTime(int resourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservation> SelectOverwritable(int resourceId, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservation> SelectPastEndableRepair()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservation> SelectPastUnstarted()
        {
            throw new NotImplementedException();
        }

        public double SelectReservableMinutes(int resourceId, int clientId, TimeSpan reservFence, TimeSpan maxAlloc, DateTime now)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservation> SelectReservationsByPeriod(DateTime period)
        {
            throw new NotImplementedException();
        }

        public void StartReservation(IReservation item, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public void UpdateAccount(int reservationId, int accountId, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public int UpdateByGroup(int groupId, DateTime sd, DateTime ed, string notes, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public void UpdateCharges(IReservation item, double chargeMultiplier, bool applyLateChargePenalty, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public void UpdateFacilityDownTime(IReservation item, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public void UpdateNotes(int reservationId, string notes)
        {
            throw new NotImplementedException();
        }

        public void UpdateReservation(IReservation item, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateReservationHistory(ReservationHistoryUpdate model)
        {
            return Put("webapi/scheduler/reservation/history", model);
        }

        public IResource GetResource(int reservationId)
        {
            return Get<ResourceItem>("webapi/scheduler/reservation/{reservationId}/resource", UrlSegments(new { reservationId }));
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

        public void CancelReservation(int reservationId, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public void CancelAndForgive(int reservationId, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public int CancelByGroup(int groupId, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public int CancelByRecurrence(int recurrenceId, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public void EndReservation(int reservationId, int? endedByClientId, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public void EndForRepair(int reservationId, int? endedByClientId, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public int EndPastUnstarted(int reservationId, DateTime ed, int? endedByClientId)
        {
            throw new NotImplementedException();
        }

        public IReservation InsertReservation(InsertReservationArgs args)
        {
            throw new NotImplementedException();
        }

        public IReservation InsertForModification(InsertReservationArgs args, IReservation linkedReservation)
        {
            throw new NotImplementedException();
        }

        public IReservation InsertFacilityDownTime(int resourceId, int clientId, int groupId, DateTime beginDateTime, DateTime endDateTime, string notes, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public void StartReservation(int reservationId, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public IReservation UpdateReservation(UpdateReservationArgs args)
        {
            throw new NotImplementedException();
        }

        public void UpdateCharges(int reservationId, double chargeMultiplier, bool applyLateChargePenalty, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public IReservation UpdateFacilityDownTime(int reservationId, DateTime beginDateTime, DateTime endDateTime, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public IReservation UpdateRepair(int reservationId, DateTime endDateTime, string notes, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public IReservation FromDataRow(DataRow dr)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservation> GetRecurringReservations(int recurrenceId, DateTime? sd, DateTime? ed)
        {
            throw new NotImplementedException();
        }

        public bool DeleteInvitee(int reservationId, int inviteeId)
        {
            throw new NotImplementedException();
        }

        public bool InviteeExists(int reservationId, int inviteeId)
        {
            throw new NotImplementedException();
        }

        public IReservationInvitee GetInvitee(int reservationId, int inviteeId)
        {
            throw new NotImplementedException();
        }

        public void AddInvitee(int reservationId, int inviteeId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservationInvitee> ToReservationInviteeList(DataTable dt, int reservationId)
        {
            throw new NotImplementedException();
        }

        public IReservationRecurrence GetReservationRecurrence(int recurrenceId)
        {
            throw new NotImplementedException();
        }

        public bool SaveReservationRecurrence(int recurrenceId, int patternId, int param1, int? param2, DateTime beginDate, TimeSpan beginTime, double duration, DateTime? endDate, bool autoEnd, bool keepAlive, string notes)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservation> SelectByDateRange(DateTime sd, DateTime ed, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public int[] FilterInvitedReservations(int[] reservationIds, int clientId)
        {
            throw new NotImplementedException();
        }

        public DateTime? GetLastRepairEndTime(int resourceId)
        {
            throw new NotImplementedException();
        }

        public double GetReservableMinutes(int resourceId, int clientId, TimeSpan reservFence, TimeSpan maxAlloc, DateTime now)
        {
            throw new NotImplementedException();
        }

        public void ExtendReservation(int reservationId, int totalMinutes, int? modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservation> SelectRecentReservations(int resourceId, int? take = null)
        {
            throw new NotImplementedException();
        }

        public IReservation GetPreviousRecurrence(int recurrenceId, int exclude = 0)
        {
            throw new NotImplementedException();
        }

        IReservationInvitee IReservationRepository.AddInvitee(int reservationId, int inviteeId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AvailableInvitee> GetAvailableInvitees(int reservationId, int resourceId, int activityId, int clientId)
        {
            throw new NotImplementedException();
        }

        public IOnTheFlyResource GetOnTheFlyResource(int resourceId)
        {
            throw new NotImplementedException();
        }

        public IReservationGroup UpdateReservationGroup(int groupId, DateTime beginDateTime, DateTime endDateTime)
        {
            throw new NotImplementedException();
        }

        public IAutoEndLog AddAutoEndLog(IReservation rsv, string action)
        {
            throw new NotImplementedException();
        }

        public int InsertReservationRecurrence(int resourceId, int clientId, int accountId, int activityId, int patternId, int param1, int? param2, DateTime beginDateTime, double duration, DateTime? endDate, bool autoEnd, bool keepAlive, string notes)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservationInviteeItem> GetInviteeItems(int reservationId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservationInvitee> GetInvitees(int[] reservations)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservationInviteeItem> SelectInviteesByResource(int resourceId, DateTime sd, DateTime ed, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservationInviteeItem> SelectInviteesByProcessTech(int processTechId, DateTime sd, DateTime ed, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservationInviteeItem> SelectInviteesByLabLocation(int labLocationId, DateTime sd, DateTime ed, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservationInviteeItem> SelectInviteesByClient(int clientId, DateTime sd, DateTime ed, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservationInviteeItem> SelectInviteesByDateRange(DateTime sd, DateTime ed, bool includeDeleted)
        {
            throw new NotImplementedException();
        }
    }
}
