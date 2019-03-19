using LNF.Models.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public class ReservationData
    {
        public ReservationData() : this(null, null) { }

        public ReservationData(IEnumerable<Models.Scheduler.ReservationProcessInfoItem> processInfos)
            : this(processInfos, null) { }

        public ReservationData(IEnumerable<ReservationInviteeItem> invitees)
            : this(null, invitees) { }

        public ReservationData(IEnumerable<Models.Scheduler.ReservationProcessInfoItem> processInfos, IEnumerable<ReservationInviteeItem> invitees)
        {
            ProcessInfos = processInfos == null ? new List<Models.Scheduler.ReservationProcessInfoItem>() : processInfos.ToList();
            Invitees = invitees == null ? new List<ReservationInviteeItem>() : invitees.ToList();
        }

        public int ClientID { get; set; }
        public int ResourceID { get; set; }
        public int ActivityID { get; set; }
        public int AccountID { get; set; }
        public string Notes { get; set; }
        public bool AutoEnd { get; set; }
        public bool KeepAlive { get; set; }
        public ReservationDuration ReservationDuration { get; set; }
        public IList<Models.Scheduler.ReservationProcessInfoItem> ProcessInfos { get; }
        public IList<ReservationInviteeItem> Invitees { get; }

        /// <summary>
        /// Updates the ReservationItem using the current instance property values. Nothing is done to the database.
        /// </summary>
        public void Update(ReservationItem item)
        {
            item.ResourceID = ResourceID;
            item.ClientID = ClientID;
            item.ActivityID = ActivityID;
            item.AccountID = AccountID;
            item.BeginDateTime = ReservationDuration.BeginDateTime;
            item.EndDateTime = ReservationDuration.EndDateTime;
            item.Duration = ReservationDuration.Duration.TotalMinutes;
            item.LastModifiedOn = DateTime.Now;
            item.Notes = Notes;
            item.AutoEnd = AutoEnd;
            item.HasProcessInfo = ProcessInfos.Count > 0;
            item.HasInvitees = Invitees.Count(x => !x.Removed) > 0;
            item.KeepAlive = KeepAlive;
        }
    }
}
