using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository;
using System;
using System.Collections.Generic;
using System.Data;

namespace LNF.Scheduler
{
    public interface IReservationManager : IManager
    {
        IEnumerable<IClientAccount> AvailableAccounts(IReservation item);
        IReservation CreateReservation(int resourceId, int clientId, int accountId, int activityId, DateTime beginDateTime, DateTime endDateTime, double duration, string notes, bool autoEnd, bool hasProcessInfo, bool hasInvitees, int? recurrenceId, bool isActive, bool keepAlive, double maxReservedDuration, int? modifiedByClientId);
        void DeleteReservation(IReservation item, int? modifiedByClientId);
        void DeleteAndForgive(IReservation item, int? modifiedByClientId);
        int DeleteByGroup(int groupId, int? modifiedByClientId);
        int DeleteByRecurrence(int recurrenceId, int? modifiedByClientId);
        void EndReservation(IReservation item, int? endedByClientId, int? modifiedByClientId);
        void EndForRepair(IReservation item, int? endedByClientId, int? modifiedByClientId);
        int EndPastUnstarted(IReservation item, DateTime endDate, int? endedByClientId);
        IEnumerable<ReservationHistoryFilterItem> FilterCancelledReservations(IEnumerable<IReservation> reservations, bool includeCanceledForModification);
        IReservation FromDataRow(DataRow dr);
        IEnumerable<IResourceClient> GetResourceClients(int resourceId);
        int GetAvailableSchedMin(int resourceId, int clientId);
        IEnumerable<IReservation> GetCurrentReservations();
        IEnumerable<IReservationHistory> GetHistory(int reservationId);
        IEnumerable<IReservationInvitee> GetInvitees(int reservationId);
        void InsertReservation(IReservation item, DateTime now, int? modifiedByClientId);
        void InsertFacilityDownTime(IReservation item, DateTime now, int? modifiedByClientId);
        void InsertForModification(IReservation item, DateTime now, int linkedReservationId, int? modifiedByClientId);
        void CanCreateCheck(IReservation item, DateTime now, int? modifiedByclientId);
        IReservation InsertRepair(int resourceId, int clientId, DateTime beginDateTime, DateTime endDateTime, DateTime actualBeginDateTime, string notes, int? modifiedByClientId);
        void AppendNotes(int reservationId, string notes);
        bool IsInvited(int reservationId, int clientId);
        IEnumerable<IReservation> ReservationsInGranularityWindow(IResource res);
        IEnumerable<IReservation> SelectAutoEnd();
        IEnumerable<IReservation> SelectByClient(int clientId, DateTime startDate, DateTime endDate, bool includeDeleted);
        IEnumerable<IReservation> SelectByDateRange(DateTime startDate, DateTime endDate, int clientId = 0);
        IEnumerable<IReservation> SelectByGroup(int groupId);
        IEnumerable<IReservation> SelectByProcessTech(int processTechId, DateTime startDate, DateTime endDate, bool includeDeleted);
        IEnumerable<IReservation> SelectByResource(int resourceId, DateTime startDate, DateTime endDate, bool includeDeleted);
        IEnumerable<IReservation> SelectEndableReservations(int resourceId);
        IEnumerable<IReservation> SelectExisting(int resourceId);
        IEnumerable<IReservation> SelectHistory(int clientId, DateTime sd, DateTime ed);
        IEnumerable<IReservation> SelectHistoryToForgiveForRepair(int resourceId, DateTime sd, DateTime ed);
        DateTime? SelectLastRepairEndTime(int resourceId);
        IEnumerable<IReservation> SelectOverwritable(int resourceId, DateTime sd, DateTime ed);
        IEnumerable<IReservation> SelectPastEndableRepair();
        IEnumerable<IReservation> SelectPastUnstarted();
        double SelectReservableMinutes(int resourceId, int clientId, TimeSpan reservFence, TimeSpan maxAlloc, DateTime now);
        IEnumerable<IReservation> SelectReservationsByPeriod(DateTime period);
        void StartReservation(IReservation item, int? modifiedByClientId);
        void UpdateReservation(IReservation item, int? modifiedByClientId);
        void UpdateAccount(int reservationId, int accountId, int? modifiedByClientId);
        int UpdateByGroup(int groupId, DateTime sd, DateTime ed, string notes, int? modifiedByClientId);
        void UpdateCharges(IReservation item, double chargeMultiplier, bool applyLateChargePenalty, int? modifiedByClientId);
        void UpdateFacilityDownTime(IReservation item, int? modifiedByClientId);
        void UpdateNotes(int reservationId, string notes);
        IReservation GetReservation(int reservationId);
        IReservationWithInvitees GetReservationWithInvitees(int reservationId);
        IEnumerable<IReservation> GetReservations(int resourceId, DateTime sd, DateTime ed, bool? isActive = null, bool? isStarted = null);
        TimeSpan GetTimeUntilNextReservation(IResource res, int reservationId, int clientId, DateTime beginDateTime);
        IEnumerable<IResource> GetResources(IEnumerable<IReservation> reservations);

        /// <summary>
        /// Updates reservation and billing data.
        /// </summary>
        SaveReservationHistoryResult SaveReservationHistory(IReservation item, int accountId, double forgivenPct, string notes, bool emailClient);

        /// <summary>
        /// Use with extreme caution! Will permanently delete a reservation from the database, along with any related invitee, process info, and history records.
        /// </summary>
        int PurgeReservation(int reservationId);

        /// <summary>
        /// Use with extreme caution! Will permanently delete reservations from the database, along with any related invitee, process info, and history records.
        /// </summary>
        int PurgeReservations(int resourceId, DateTime sd, DateTime ed);
    }
}