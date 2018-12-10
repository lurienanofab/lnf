using LNF.Models.Data;
using System;
using System.Collections.Generic;

namespace LNF.Models.Scheduler
{
    public class ReservationItem : IPrivileged
    {
        public int ReservationID { get; set; }
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public int MinCancelTime { get; set; }
        public int MinReservTime { get; set; }
        public int ProcessTechID { get; set; }
        public int LabID { get; set; }
        public int BuildingID { get; set; }
        public int ClientID { get; set; }
        public string UserName { get; set; }
        public string LName { get; set; }
        public string FName { get; set; }
        public ClientPrivilege Privs { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string ShortCode { get; set; }
        public int ActivityID { get; set; }
        public string ActivityName { get; set; }
        public ActivityAccountType ActivityAccountType { get; set; }
        public ClientAuthLevel StartEndAuth { get; set; }
        public bool Editable { get; set; }
        public bool IsFacilityDownTime { get; set; }
        public DateTime BeginDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime? ActualBeginDateTime { get; set; }
        public DateTime? ActualEndDateTime { get; set; }
        public DateTime ChargeBeginDateTime { get; set; }
        public DateTime ChargeEndDateTime { get; set; }
        public int? ClientIDBegin { get; set; }
        public int? ClientIDEnd { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public double Duration { get; set; }
        public string Notes { get; set; }
        public double ChargeMultiplier { get; set; }
        public bool ApplyLateChargePenalty { get; set; }
        public bool AutoEnd { get; set; }
        public bool HasProcessInfo { get; set; }
        public bool HasInvitees { get; set; }
        public bool IsActive { get; set; }
        public bool IsStarted { get; set; }
        public bool IsUnloaded { get; set; }
        public int? RecurrenceID { get; set; }
        public int? GroupID { get; set; }
        public double MaxReservedDuration { get; set; }
        public DateTime? CancelledDateTime { get; set; }
        public bool KeepAlive { get; set; }
        public DateTime? OriginalBeginDateTime { get; set; }
        public DateTime? OriginalEndDateTime { get; set; }
        public DateTime? OriginalModifiedOn { get; set; }        

        public string GetResourceDisplayName() => ResourceItem.GetDisplayName(ResourceName, ResourceID);

        public double ReservedDuration() => (EndDateTime - BeginDateTime).TotalMinutes;

        public double ActualDuration() => (ActualBeginDateTime != null && ActualEndDateTime != null) ? (ActualEndDateTime.Value - ActualBeginDateTime.Value).TotalMinutes : 0;

        public double ChargeDuration() => (ChargeEndDateTime - ChargeBeginDateTime).TotalMinutes;

        public double Overtime() => (ActualEndDateTime != null) ? Math.Max((ActualEndDateTime.Value - EndDateTime).TotalMinutes, 0) : 0;
    }

    public class ReservationWithInvitees : ReservationItem
    {
        public IEnumerable<ReservationInviteeItem> Invitees { get; set; }
    }
}
