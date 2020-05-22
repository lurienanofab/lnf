﻿using LNF.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace LNF.Scheduler
{
    public interface IReservationRepository
    {
        IReservation GetReservation(int reservationId);
        IEnumerable<IReservation> GetReservations(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0, int activityId = 0, bool? started = null, bool? active = null);
        IReservationWithInvitees GetReservationWithInvitees(int reservationId);
        IEnumerable<IReservationWithInvitees> GetReservationsWithInvitees(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0, int activityId = 0, bool? started = null, bool? active = null);
        IEnumerable<IClientAccount> AvailableAccounts(IReservation rsv);
        IReservation CreateReservation(int resourceId, int clientId, int accountId, int activityId, DateTime beginDateTime, DateTime endDateTime, double duration, string notes, bool autoEnd, bool hasProcessInfo, bool hasInvitees, int? recurrenceId, bool isActive, bool keepAlive, double maxReservedDuration, int? modifiedByClientId);
        void CancelReservation(int reservationId, int? modifiedByClientId);
        void CancelAndForgive(int reservationId, int? modifiedByClientId);
        int CancelByGroup(int groupId, int? modifiedByClientId);
        int CancelByRecurrence(int recurrenceId, int? modifiedByClientId);
        void EndReservation(EndReservationArgs args);
        void EndForRepair(int reservationId, int? endedByClientId, int? modifiedByClientId);
        int EndPastUnstarted(int reservationId, DateTime endDate, int? endedByClientId);
        IEnumerable<ReservationHistoryFilterItem> FilterCancelledReservations(IEnumerable<IReservation> reservations, bool includeCanceledForModification);
        IEnumerable<IResourceClient> GetResourceClients(int resourceId);
        int GetAvailableSchedMin(int resourceId, int clientId);
        IEnumerable<IReservation> GetCurrentReservations();
        IEnumerable<IReservationHistory> GetHistory(int reservationId);
        IReservation InsertReservation(InsertReservationArgs args);
        IReservation InsertForModification(InsertReservationArgs args);
        IReservation InsertFacilityDownTime(int resourceId, int clientId, int groupId, DateTime beginDateTime, DateTime endDateTime, string notes, int? modifiedByClientId);
        IReservation InsertRepair(int resourceId, int clientId, DateTime beginDateTime, DateTime endDateTime, DateTime actualBeginDateTime, string notes, int? modifiedByClientId);
        void AppendNotes(int reservationId, string notes);
        bool IsInvited(int reservationId, int clientId);
        IEnumerable<IReservation> ReservationsInGranularityWindow(IResource res);
        IEnumerable<IReservation> SelectAutoEnd();
        IEnumerable<IReservation> SelectByClient(int clientId, DateTime sd, DateTime ed, bool includeDeleted);
        IEnumerable<IReservation> SelectByDateRange(DateTime sd, DateTime ed, bool includeDeleted);
        IEnumerable<IReservation> SelectByProcessTech(int processTechId, DateTime sd, DateTime ed, bool includeDeleted);
        IEnumerable<IReservation> SelectByLabLocation(int labLocationId, DateTime sd, DateTime ed, bool includeDeleted);
        IEnumerable<IReservation> SelectByResource(int resourceId, DateTime sd, DateTime ed, bool includeDeleted);
        IEnumerable<IReservation> SelectByGroup(int groupId);
        IEnumerable<IReservation> SelectEndableReservations(int resourceId);
        IEnumerable<IReservation> SelectExisting(int resourceId);
        IEnumerable<IReservation> SelectHistory(int clientId, DateTime sd, DateTime ed);
        IEnumerable<IReservation> SelectHistoryToForgiveForRepair(int resourceId, DateTime sd, DateTime ed);
        IEnumerable<IReservation> SelectOverwritable(int resourceId, DateTime sd, DateTime ed);
        IEnumerable<IReservation> SelectPastEndableRepair();
        IEnumerable<IReservation> SelectPastUnstarted();
        IEnumerable<IReservation> SelectReservationsByPeriod(DateTime period);
        DateTime? GetLastRepairEndTime(int resourceId);
        double GetReservableMinutes(int resourceId, int clientId, TimeSpan reservFence, TimeSpan maxAlloc, DateTime now);
        void StartReservation(int reservationId, int? modifiedByClientId);
        IReservation UpdateReservation(UpdateReservationArgs args);
        void UpdateAccount(int reservationId, int accountId, int? modifiedByClientId);
        int UpdateByGroup(int groupId, DateTime sd, DateTime ed, string notes, int? modifiedByClientId);
        void UpdateCharges(int reservationId, double chargeMultiplier, bool applyLateChargePenalty, int? modifiedByClientId);
        IReservation UpdateFacilityDownTime(int reservationId, DateTime beginDateTime, DateTime endDateTime, int? modifiedByClientId);
        IReservation UpdateRepair(int reservationId, DateTime endDateTime, string notes, int? modifiedByClientId);
        void UpdateNotes(int reservationId, string notes);
        bool UpdateReservationHistory(ReservationHistoryUpdate model);
        IReservation FromDataRow(DataRow dr);
        TimeSpan GetTimeUntilNextReservation(IResource res, int reservationId, int clientId, DateTime beginDateTime);
        IEnumerable<IResource> GetResources(IEnumerable<IReservation> reservations);

        /// <summary>
        /// Updates reservation and billing data.
        /// </summary>
        SaveReservationHistoryResult SaveReservationHistory(IReservation rsv, int accountId, double forgivenPct, string notes, bool emailClient);

        /// <summary>
        /// Use with extreme caution! Will permanently delete a reservation from the database, along with any related invitee, process info, and history records.
        /// </summary>
        int PurgeReservation(int reservationId);

        /// <summary>
        /// Use with extreme caution! Will permanently delete reservations from the database, along with any related invitee, process info, and history records.
        /// </summary>
        int PurgeReservations(int resourceId, DateTime sd, DateTime ed);

        IReservationRecurrence GetReservationRecurrence(int recurrenceId);

        /// <summary>
        /// Gets the most recent instance of a recurring reservation.
        /// </summary>
        /// <param name="recurrenceId">The recurrence.</param>
        /// <param name="exclude">A ReservationID to exclude. Use zero to exclude no reservations.</param>
        IReservation GetPreviousRecurrence(int recurrenceId, int exclude = 0);

        IEnumerable<IReservationRecurrence> GetReservationRecurrencesByResource(int resourceId);
        IEnumerable<IReservationRecurrence> GetReservationRecurrencesByProcessTech(int processTechId);
        IEnumerable<IReservationRecurrence> GetReservationRecurrencesByLabLocation(int labLocationId);
        IEnumerable<IReservationRecurrence> GetReservationRecurrencesByClient(int clientId);
        bool SaveReservationRecurrence(int recurrenceId, int patternId, int param1, int? param2, DateTime beginDate, TimeSpan beginTime, double duration, DateTime? endDate, bool autoEnd, bool keepAlive, string notes);
        IEnumerable<IReservation> GetRecurringReservations(int recurrenceId, DateTime? sd, DateTime? ed);
        IResource GetResource(int reservationId);
        IEnumerable<ReservationStateItem> GetReservationStates(DateTime sd, DateTime ed, string kioskIp, int? clientId = null, int? resourceId = null, int? reserverId = null);

        /// <summary>
        /// Adds a ReservationInvitee record if it does not already exist.
        /// </summary>
        IReservationInvitee AddInvitee(int reservationId, int inviteeId);

        /// <summary>
        /// Deletes an existing ReservationInvitee record.
        /// </summary>
        void DeleteInvitee(int reservationId, int inviteeId);

        bool InviteeExists(int reservationId, int inviteeId);

        IEnumerable<IReservationInvitee> GetInvitees(int reservationId);

        IReservationInvitee GetInvitee(int reservationId, int inviteeId);

        IEnumerable<IAvailableInvitee> GetAvailableInvitees(int reservationId, int resourceId, int activityId, int clientId);

        /// <summary>
        /// Returns the ReservationIDs in the given array to which the given client was invited.
        /// </summary>
        int[] FilterInvitedReservations(int[] reservationIds, int clientId);

        void ExtendReservation(int reservationId, int totalMinutes, int? modifiedByClientId);
        IEnumerable<IReservation> SelectRecentReservations(int resourceId, int? take = null);
        IReservationGroup UpdateReservationGroup(int groupId, DateTime beginDateTime, DateTime endDateTime);
        IAutoEndLog AddAutoEndLog(IReservation rsv, string action);
    }
}