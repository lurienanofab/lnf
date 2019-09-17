using LNF.Models.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public class ReservationData
    {
        public ReservationData() : this(null, null) { }

        public ReservationData(IEnumerable<IReservationProcessInfo> processInfos)
            : this(processInfos, null) { }

        public ReservationData(IEnumerable<IReservationInvitee> invitees)
            : this(null, invitees) { }

        public ReservationData(IEnumerable<IReservationProcessInfo> processInfos, IEnumerable<IReservationInvitee> invitees)
        {
            ProcessInfos = processInfos == null ? new List<IReservationProcessInfo>() : processInfos.ToList();
            Invitees = invitees == null ? new List<IReservationInvitee>() : invitees.ToList();
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
        public IList<IReservationProcessInfo> ProcessInfos { get; }
        public IList<IReservationInvitee> Invitees { get; }

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
                Now = now,
                ModifiedByClientID = modifiedByClientId.GetValueOrDefault(ClientID)
            };
        }
    }
}
