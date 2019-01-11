using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Scheduler
{
    public interface IReservationManager : IManager
    {
        ReservationHistory AddReservationHistory(string actionSource, string userAction, Reservation rsv, Client modifiedBy = null);
        IList<ClientAccount> AvailableAccounts(ReservationItem rsv);
        void CanCreateCheck(Reservation rsv);
        DataTable CopyReservationInviteesTable();
        DataTable CopyReservationProcessInfoTable();
        Reservation Create(Resource resource, Client client, Account account, Activity activity, DateTime beginDateTime, DateTime endDateTime, double duration, string notes, bool autoEnd, bool hasProcessInfo, bool hasInvitees, ReservationRecurrence recurrence, bool isActive, bool keepAlive, double maxReservedDuration, Client modifiedBy);
        void Delete(Reservation rsv, int? modifiedByClientId);
        void DeleteAndForgive(Reservation rsv, int? modifiedByClientId);
        void DeleteReservation(int reservationId);
        int DeleteByGroup(int groupId, int? modifiedByClientId);
        int DeleteByRecurrence(int recurrenceId, int? modifiedByClientId);
        void End(Reservation rsv, int? endedByClientId, int? modifiedByClientId);
        HandleAutoEndReservationsProcessResult HandleAutoEndReservations(IEnumerable<Reservation> reservations);
        void EndForRepair(Reservation rsv, int? endedByClientId, int? modifiedByClientId);
        int EndPastUnstarted(Reservation rsv, DateTime endDate, int clientId);
        HandleRepairReservationsProcessResult HandleRepairReservations(IEnumerable<Reservation> reservations);
        void EndReservation(int reservationId);
        HandleUnstartedReservationsProcessResult HandleUnstartedReservations(IEnumerable<Reservation> reservations);
        IList<ReservationHistoryFilterItem> FilterCancelledReservations(IList<Reservation> reservations, bool includeCanceledForModification);
        Reservation FromDataRow(DataRow dr);
        ClientAuthLevel GetAuthLevel(Reservation rsv, IPrivileged client);
        ClientAuthLevel GetAuthLevel(IEnumerable<IAuthorized> resourceClients, IPrivileged client);
        int GetAvailableSchedMin(int resourceId, int clientId);
        IList<Reservation> GetConflictingReservations(IEnumerable<Reservation> reservations, DateTime sd, DateTime ed);
        IList<Reservation> GetCurrentReservations();
        IQueryable<ReservationHistory> GetHistory(Reservation rsv);
        IQueryable<ReservationInvitee> GetInvitees(int reservationId);
        ReservationInProgress GetRepairReservationInProgress(int resourceId);
        string GetReservationCaption(ReservationState state);
        Reservation GetReservationForModification(Reservation rsv, ReservationDuration rd, out bool insert);
        ReservationStateArgs CreateReservationStateArgs(Reservation rsv, Client client, string kioskIp);
        ReservationState GetReservationState(ReservationStateArgs args);
        string GetReservationToolTip(Reservation rsv, ReservationState state);
        bool HandleFacilityDowntimeResrvation(Reservation rsv);
        bool HandlePracticeReservation(Reservation rsv);
        void Insert(Reservation rsv, int? modifiedByClientId);
        void InsertFacilityDownTime(Reservation rsv, int? modifiedByClientId);
        void InsertForModification(Reservation rsv, int linkedReservationId, int? modifiedByClientId);
        Reservation InsertRepair(int resourceId, int clientId, DateTime beginDateTime, DateTime endDateTime, DateTime actualBeginDateTime, string notes, int? modifiedByClientId);
        ReservationHistory InsertReservationHistory(string action, string actionSource, Reservation rsv, int? modifiedByClientId = null, int? linkedReservationId = null);
        bool IsInvited(int reservationId, int clientId);
        bool IsStartable(DateTime now, DateTime beginDateTime, int minReservTime);
        bool IsStartable(DateTime beginDateTime, int minReservTime);
        bool IsStartable(ReservationState state);
        Reservation ModifyExistingReservation(Reservation rsv, ReservationDuration rd, IList<ReservationInviteeItem> invitees);
        DateTime? OpenResSlot(int resourceId, TimeSpan reservFence, TimeSpan minReservTime, DateTime now, DateTime sd);
        IList<Reservation> ReservationsInWindow(Resource resource, int minutes);
        IList<Reservation> SelectAutoEnd();
        IList<Reservation> SelectByClient(int clientId, DateTime startDate, DateTime endDate, bool includeDeleted);
        IList<Reservation> SelectByDateRange(DateTime startDate, DateTime endDate, int clientId = 0);
        IList<Reservation> SelectByGroup(int groupId);
        IList<Reservation> SelectByProcessTech(int processTechId, DateTime startDate, DateTime endDate, bool includeDeleted);
        IList<Reservation> SelectByResource(int resourceId, DateTime startDate, DateTime endDate, bool includeDeleted);
        IList<Reservation> SelectEndableReservations(int resourceId);
        IList<Reservation> SelectExisting(Resource resource);
        IList<Reservation> SelectHistory(int clientId, DateTime sd, DateTime ed);
        IList<Reservation> SelectHistoryToForgiveForRepair(int resourceId, DateTime sd, DateTime ed);
        DateTime? SelectLastRepairEndTime(int resourceId);
        IList<Reservation> SelectOverwrittable(int resourceId, DateTime sd, DateTime ed);
        IList<Reservation> SelectPastEndableRepair();
        IList<Reservation> SelectPastUnstarted();
        double SelectReservableMinutes(int resourceId, int clientId, TimeSpan reservFence, TimeSpan maxAlloc, DateTime now);
        IList<Reservation> SelectReservationsByPeriod(DateTime period);
        void Start(Reservation rsv, int? startedByClientId, int? modifiedByClientId);
        void StartReservation(Reservation rsv, Client client, string kioskIp);
        void Update(Reservation rsv, int? modifiedByClientId);
        void UpdateAccount(Reservation rsv, int accountId);
        int UpdateByGroup(int groupId, DateTime sd, DateTime ed, string notes, int? modifiedByClientId);
        void UpdateCharges(Reservation rsv, double chargeMultiplier);
        void UpdateCharges(Reservation rsv, double chargeMultiplier, bool applyLateChargePenalty, int? modifiedByClientId);
        void UpdateFacilityDownTime(Reservation rsv, int? modifiedByClientId);
        void UpdateNotes(Reservation rsv, string notes);
        void UpdateReservation(Reservation rsv, ReservationDuration rd);
        TimeSpan GetTimeUntilNextReservation(int resourceId, int reservationId, int clientId, DateTime beginDateTime);

        /// <summary>
        /// Updates reservation and billing data.
        /// </summary>
        SaveReservationHistoryResult SaveReservationHistory(ReservationItem rsv, int accountId, double forgivenPct, string notes, bool emailClient);

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