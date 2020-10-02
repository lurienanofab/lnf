using LNF.Data;
using System;
using System.Collections.Generic;

namespace LNF.Scheduler
{
    public interface IReservation : IResource, IPrivileged, IClientOrg
    {
        int ReservationID { get; set; }
        int AccountID { get; set; }
        string AccountName { get; set; }
        string ShortCode { get; set; }
        int ChargeTypeID { get; set; }
        int ActivityID { get; set; }
        string ActivityName { get; set; }
        ActivityAccountType ActivityAccountType { get; set; }
        ClientAuthLevel StartEndAuth { get; set; }
        bool Editable { get; set; }
        bool IsRepair { get; }
        bool IsFacilityDownTime { get; set; }
        string LName { get; set; }
        string FName { get; set; }
        string DisplayName { get; }
        DateTime BeginDateTime { get; set; }
        DateTime EndDateTime { get; set; }
        DateTime? ActualBeginDateTime { get; set; }
        DateTime? ActualEndDateTime { get; set; }
        DateTime ChargeBeginDateTime { get; }
        DateTime ChargeEndDateTime { get; }
        int? ClientIDBegin { get; set; }
        int? ClientIDEnd { get; set; }
        DateTime CreatedOn { get; set; }
        DateTime LastModifiedOn { get; set; }
        double Duration { get; set; }
        string Notes { get; set; }
        double ChargeMultiplier { get; set; }
        bool ApplyLateChargePenalty { get; set; }
        bool AutoEnd { get; set; }
        bool HasProcessInfo { get; set; }
        bool HasInvitees { get; set; }
        bool IsActive { get; set; }
        bool IsStarted { get; set; }
        bool IsUnloaded { get; set; }
        int? RecurrenceID { get; set; }
        int? GroupID { get; set; }
        double MaxReservedDuration { get; set; }
        DateTime? CancelledDateTime { get; set; }
        bool KeepAlive { get; set; }
        DateTime? OriginalBeginDateTime { get; set; }
        DateTime? OriginalEndDateTime { get; set; }
        DateTime? OriginalModifiedOn { get; set; }
        TimeSpan GetReservedDuration();
        TimeSpan GetActualDuration();
        TimeSpan GetChargeDuration();
        TimeSpan GetOvertimeDuration();
        bool IsRunning { get; }
        bool IsCancelledBeforeCutoff { get; }
        bool IsCurrentlyOutsideGracePeriod { get; }
    }

    public interface IReservationWithInvitees : IReservation
    {
        IEnumerable<IReservationInvitee> Invitees { get; set; }
    }
}
