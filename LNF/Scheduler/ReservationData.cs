using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public class ReservationData
    {
        public ReservationData() : this(null, null) { }

        public ReservationData(IEnumerable<ReservationProcessInfoItem> processInfos)
            : this(processInfos, null) { }

        public ReservationData(IEnumerable<Invitee> invitees)
            : this(null, invitees) { }

        public ReservationData(IEnumerable<ReservationProcessInfoItem> processInfos, IEnumerable<Invitee> invitees)
        {
            ProcessInfos = processInfos == null ? new List<ReservationProcessInfoItem>() : processInfos.ToList();
            Invitees = invitees == null ? new List<Invitee>() : invitees.ToList();
        }

        public int ClientID { get; set; }
        public int ResourceID { get; set; }
        public int ActivityID { get; set; }
        public int AccountID { get; set; }
        public int? RecurrenceID { get; set; }
        public string Notes { get; set; }
        public bool AutoEnd { get; set; }
        public bool KeepAlive { get; set; }
        public ReservationDuration Duration { get; set; }
        public IList<ReservationProcessInfoItem> ProcessInfos { get; }
        public IList<Invitee> Invitees { get; }

        /// <summary>
        /// Creates an instance of InsertReservationArgs with the current ReservationData object. Nothing is done to the database.
        /// </summary>
        public InsertReservationArgs CreateInsertArgs(DateTime now, int linkedReservationId, int? modifiedByClientId = null)
        {
            return new InsertReservationArgs
            {
                ClientID = ClientID,
                AccountID = AccountID,
                ResourceID = ResourceID,
                ActivityID = ActivityID,
                RecurrenceID = RecurrenceID.GetValueOrDefault(-1), //always -1 for non-recurring reservation
                BeginDateTime = Duration.BeginDateTime,
                EndDateTime = Duration.EndDateTime,
                ChargeMultiplier = 1,
                AutoEnd = AutoEnd,
                KeepAlive = KeepAlive,
                HasInvitees = Invitees.Any(x => !x.Removed),
                HasProcessInfo = ProcessInfos.Any(),
                Notes = Notes,
                Now = now,
                LinkedReservationID = linkedReservationId,
                ModifiedByClientID = modifiedByClientId.GetValueOrDefault(ClientID)
            };
        }

        public UpdateReservationArgs CreateUpdateArgs(DateTime now, int reservationId, int? modifiedByClientId = null)
        {
            return new UpdateReservationArgs
            {
                ReservationID = reservationId,
                AccountID = AccountID,
                AutoEnd = AutoEnd,
                KeepAlive = KeepAlive,
                HasInvitees = Invitees.Any(x => !x.Removed),
                HasProcessInfo = ProcessInfos.Any(),
                Notes = Notes,
                ChargeMultiplier = 1, // any update from Reservation.aspx occurs before a reservation would ever be forgiven
                Now = now,
                ModifiedByClientID = modifiedByClientId.GetValueOrDefault(ClientID)
            };
        }
    }
}
