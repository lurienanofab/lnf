using LNF.Data;
using LNF.Scheduler;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Scheduler
{
    public class ReservationItem : IReservationItem
    {
        public int ReservationID { get; set; }
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public string ResourceDisplayName => Resources.GetResourceDisplayName(ResourceName, ResourceID);
        public int ReservFence { get; set; }
        public int Granularity { get; set; }
        public int Offset { get; set; }
        public int MinReservTime { get; set; }
        public int MaxReservTime { get; set; }
        public int MinCancelTime { get; set; }
        public int GracePeriod { get; set; }
        public int ResourceAutoEnd { get; set; }
        public int AuthDuration { get; set; }
        public bool AuthState { get; set; }
        public int ProcessTechID { get; set; }
        public string ProcessTechName { get; set; }
        public int LabID { get; set; }
        public int BuildingID { get; set; }
        public int ActivityID { get; set; }
        public string ActivityName { get; set; }
        public bool Editable { get; set; }
        public bool IsRepair => !Editable;
        public ActivityAccountType ActivityAccountType { get; set; }
        public ClientAuthLevel StartEndAuth { get; set; }
        public bool IsFacilityDownTime { get; set; }
        public int ClientID { get; set; }
        public string UserName { get; set; }
        public ClientPrivilege Privs { get; set; }
        public string LName { get; set; }
        public string MName { get; set; }
        public string FName { get; set; }
        public string DisplayName => Clients.GetDisplayName(LName, FName);
        public string Email { get; set; }
        public string Phone { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string ShortCode { get; set; }
        public int ChargeTypeID { get; set; }
        public DateTime BeginDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime? ActualBeginDateTime { get; set; }
        public DateTime? ActualEndDateTime { get; set; }
        public DateTime ChargeBeginDateTime => Reservations.GetChargeBeginDateTime(BeginDateTime, ActualBeginDateTime);
        public DateTime ChargeEndDateTime => Reservations.GetChargeEndDateTime(EndDateTime, ActualEndDateTime);
        public int? ClientIDBegin { get; set; }
        public int? ClientIDEnd { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public double Duration { get; set; }
        public string Notes { get; set; }
        public double ChargeMultiplier { get; set; }
        public bool ApplyLateChargePenalty { get; set; }
        public bool ReservationAutoEnd { get; set; }
        public bool HasProcessInfo { get; set; }
        public bool HasInvitees { get; set; }
        public bool IsActive { get; set; }
        public bool IsStarted { get; set; }
        public int? RecurrenceID { get; set; }
        public int? GroupID { get; set; }
        public DateTime? CancelledDateTime { get; set; }
        public bool KeepAlive { get; set; }
        public bool IsCancelledBeforeCutoff => Reservations.GetIsCancelledBeforeCutoff(CancelledDateTime, BeginDateTime);
        public bool IsCurrentlyOutsideGracePeriod => Reservations.GetIsCurrentlyOutsideGracePeriod(GracePeriod, BeginDateTime);
        public bool IsRunning => Reservations.GetIsRunning(ActualBeginDateTime, ActualEndDateTime);
        public TimeSpan GetActualDuration() => Reservations.GetActualDuration(ActualBeginDateTime, ActualEndDateTime);
        public TimeSpan GetChargeDuration() => Reservations.GetDuration(ChargeBeginDateTime, ChargeEndDateTime);
        public DateTime GetNextGranularity(DateTime now, GranularityDirection dir) => Resources.GetNextGranularity(Granularity, Offset, now, dir);
        public TimeSpan GetOvertimeDuration() => Reservations.GetOvertimeDuration(EndDateTime, ActualEndDateTime);
        public TimeSpan GetReservedDuration() => Reservations.GetDuration(BeginDateTime, EndDateTime);
    }

    public class ReservationWithInviteesItem : ReservationItem, IReservationWithInviteesItem
    {
        public IEnumerable<IReservationInviteeItem> Invitees { get; set; }
    }
}
