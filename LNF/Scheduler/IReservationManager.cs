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
        IEnumerable<ClientAccountItem> AvailableAccounts(ReservationItem item);
        void CanCreateCheck(ReservationItem item, int? modifiedByclientId);
        DataTable CopyReservationInviteesTable();
        DataTable CopyReservationProcessInfoTable();
        ReservationItem CreateReservation(int resourceId, int clientId, int accountId, int activityId, DateTime beginDateTime, DateTime endDateTime, double duration, string notes, bool autoEnd, bool hasProcessInfo, bool hasInvitees, int? recurrenceId, bool isActive, bool keepAlive, double maxReservedDuration, int? modifiedByClientId);
        void DeleteReservation(ReservationItem item, int? modifiedByClientId);
        void DeleteAndForgive(ReservationItem item, int? modifiedByClientId);
        int DeleteByGroup(int groupId, int? modifiedByClientId);
        int DeleteByRecurrence(int recurrenceId, int? modifiedByClientId);
        void EndReservation(ReservationItem item, int? endedByClientId, int? modifiedByClientId);
        void EndForRepair(ReservationItem item, int? endedByClientId, int? modifiedByClientId);
        int EndPastUnstarted(ReservationItem item, DateTime endDate, int? endedByClientId);
        IEnumerable<ReservationHistoryFilterItem> FilterCancelledReservations(IEnumerable<ReservationItem> reservations, bool includeCanceledForModification);
        ReservationItem FromDataRow(DataRow dr);
        IEnumerable<ResourceClientItem> GetResourceClients(int resourceId);
        int GetAvailableSchedMin(int resourceId, int clientId);
        IEnumerable<ReservationItem> GetCurrentReservations();
        IEnumerable<ReservationHistoryItem> GetHistory(int reservationId);
        IEnumerable<ReservationInviteeItem> GetInvitees(int reservationId);
        void InsertReservation(ReservationItem item, int? modifiedByClientId);
        void InsertFacilityDownTime(ReservationItem item, int? modifiedByClientId);
        void InsertForModification(ReservationItem item, int linkedReservationId, int? modifiedByClientId);
        ReservationItem InsertRepair(int resourceId, int clientId, DateTime beginDateTime, DateTime endDateTime, DateTime actualBeginDateTime, string notes, int? modifiedByClientId);
        void AppendNotes(int reservationId, string notes);
        bool IsInvited(int reservationId, int clientId);
        IEnumerable<ReservationItem> ReservationsInGranularityWindow(IResource res);
        IEnumerable<ReservationItem> SelectAutoEnd();
        IEnumerable<ReservationItem> SelectByClient(int clientId, DateTime startDate, DateTime endDate, bool includeDeleted);
        IEnumerable<ReservationItem> SelectByDateRange(DateTime startDate, DateTime endDate, int clientId = 0);
        IEnumerable<ReservationItem> SelectByGroup(int groupId);
        IEnumerable<ReservationItem> SelectByProcessTech(int processTechId, DateTime startDate, DateTime endDate, bool includeDeleted);
        IEnumerable<ReservationItem> SelectByResource(int resourceId, DateTime startDate, DateTime endDate, bool includeDeleted);
        IEnumerable<ReservationItem> SelectEndableReservations(int resourceId);
        IEnumerable<ReservationItem> SelectExisting(int resourceId);
        IEnumerable<ReservationItem> SelectHistory(int clientId, DateTime sd, DateTime ed);
        IEnumerable<ReservationItem> SelectHistoryToForgiveForRepair(int resourceId, DateTime sd, DateTime ed);
        DateTime? SelectLastRepairEndTime(int resourceId);
        IEnumerable<ReservationItem> SelectOverwritable(int resourceId, DateTime sd, DateTime ed);
        IEnumerable<ReservationItem> SelectPastEndableRepair();
        IEnumerable<ReservationItem> SelectPastUnstarted();
        double SelectReservableMinutes(int resourceId, int clientId, TimeSpan reservFence, TimeSpan maxAlloc, DateTime now);
        IEnumerable<ReservationItem> SelectReservationsByPeriod(DateTime period);
        void StartReservation(ReservationItem item, int? modifiedByClientId);
        void UpdateReservation(ReservationItem item, int? modifiedByClientId);
        void UpdateAccount(int reservationId, int accountId, int? modifiedByClientId);
        int UpdateByGroup(int groupId, DateTime sd, DateTime ed, string notes, int? modifiedByClientId);
        void UpdateCharges(ReservationItem item, double chargeMultiplier, bool applyLateChargePenalty, int? modifiedByClientId);
        void UpdateFacilityDownTime(ReservationItem item, int? modifiedByClientId);
        void UpdateNotes(int reservationId, string notes);
        ReservationItem GetReservation(int reservationId);
        ReservationItemWithInvitees GetReservationWithInvitees(int reservationId);
        TimeSpan GetTimeUntilNextReservation(IResource res, int reservationId, int clientId, DateTime beginDateTime);
        IEnumerable<ResourceItem> GetResources(IEnumerable<ReservationItem> reservations);

        /// <summary>
        /// Updates reservation and billing data.
        /// </summary>
        SaveReservationHistoryResult SaveReservationHistory(ReservationItem item, int accountId, double forgivenPct, string notes, bool emailClient);

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