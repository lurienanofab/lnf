using LNF.Repository.Data;
using System;
using System.Collections.Generic;

namespace LNF.Repository.Scheduler
{
    public interface ISchedulerRepositoryCollection
    {
        IRepository<Activity> Activity { get; }
        IRepository<Building> Building { get; }
        ILabRepository Lab { get; }
        IRepository<ProcessTech> ProcessTech { get; }
        IResourceRepository Resource { get; }
        IReservationHistoryRepository ReservationHistory { get; }
        IReservationRepository Reservation { get; }
        IReservationInviteeRepository ReservationInvitee { get; }
    }

    public interface IResourceRepository : IRepository<Resource>
    {
        IList<Resource> SelectActive();

        IList<Resource> SelectByLab(int? labId);
    }

    public interface IReservationRepository : IRepository<Reservation>
    {
        Reservation GetRepairReservationInProgress(int resourceId);
        Reservation InsertRepair(int resourceId, int clientId, DateTime beginDateTime, DateTime endDateTime, DateTime actualBeginDateTime, string notes, int? modifiedByClientId);
        int DeleteByGroup(int groupId, int? modifiedByClientId);
        int DeleteByRecurrence(int recurrenceId, int? modifiedByClientId);

        /// <summary>
        /// Gets the time before the next reservation.
        /// </summary>
        TimeSpan GetTimeUntilNextReservation(int resourceId, int reservationId, int clientId, DateTime beginDateTime);

        IList<Reservation> SelectEndableReservations(int resourceId);
        IList<Reservation> SelectByResource(int resourceId, DateTime startDate, DateTime endDate, bool includeDeleted);
        IList<Reservation> SelectByProcessTech(int procTechId, DateTime startDate, DateTime endDate, bool includeDeleted);
        IList<Reservation> SelectByClient(int clientId, DateTime startDate, DateTime endDate, bool includeDeleted);
        IList<Reservation> SelectRecent(int resourceId);
    }

    public interface IReservationHistoryRepository : IRepository<ReservationHistory>
    {
        ReservationHistory Insert(string action, string actionSource, Reservation rsv, int? modifiedByClientId = null, int? linkedReservationId = null);
    }

    public interface IReservationInviteeRepository : IRepository<ReservationInvitee>
    {
        /// <summary>
        /// Gets a list of all ClientIDs that can be invited to a reservation based on the InviteeAuth setting of the Activity, the Resource, and excluding any users already invited to the Reservation.
        /// </summary>
        /// <param name="reservationId">The ReservationID: -1 indicates a new reservation.</param>
        /// <param name="resourceId">The ResourceID</param>
        /// <param name="activityId">The ActivityID</param>
        /// <param name="clientId">The current ClientID: used to exclude the current user from the list of available invitees.</param>
        /// <returns>An array where each element is the ClientID of a user who can be invited to a reservation.</returns>
        int[] SelectAvailable(int reservationId, int resourceId, int activityId, int clientId);
    }

    public interface ILabRepository : IRepository<Lab>
    {
        IList<Lab> SelectActive();
    }
}
