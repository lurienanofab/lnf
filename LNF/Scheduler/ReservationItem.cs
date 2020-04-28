using LNF.Data;
using System;
using System.Collections.Generic;

namespace LNF.Scheduler
{
    public class ReservationItem : ResourceItem, IReservation
    {
        public int ReservationID { get; set; }
        public int ClientID { get; set; }
        public string UserName { get; set; }
        public string LName { get; set; }
        public string MName { get; set; }
        public string FName { get; set; }
        public string DisplayName => Clients.GetDisplayName(LName, FName);
        public ClientPrivilege Privs { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string ShortCode { get; set; }
        public int ChargeTypeID { get; set; }
        public int ActivityID { get; set; }
        public string ActivityName { get; set; }
        public ActivityAccountType ActivityAccountType { get; set; }
        public ClientAuthLevel StartEndAuth { get; set; }
        public bool Editable { get; set; }
        public bool IsRepair => !Editable;
        public bool IsFacilityDownTime { get; set; }
        public DateTime BeginDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime? ActualBeginDateTime { get; set; }
        public DateTime? ActualEndDateTime { get; set; }
        public DateTime ChargeBeginDateTime => GetChargeBeginDateTime(BeginDateTime, ActualBeginDateTime);
        public DateTime ChargeEndDateTime => GetChargeEndDateTime(EndDateTime, ActualEndDateTime);
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
        public TimeSpan GetReservedDuration() => GetDuration(BeginDateTime, EndDateTime);
        public TimeSpan GetActualDuration() => GetActualDuration(ActualBeginDateTime, ActualEndDateTime);
        public TimeSpan GetChargeDuration() => GetDuration(ChargeBeginDateTime, ChargeEndDateTime);
        public TimeSpan GetOvertimeDuration() => GetOvertimeDuration(EndDateTime, ActualEndDateTime);
        public bool IsRunning => GetIsRunning(ActualBeginDateTime, ActualEndDateTime);
        public bool IsCancelledBeforeCutoff => GetIsCancelledBeforeCutoff(CancelledDateTime, BeginDateTime);
        public bool IsCurrentlyOutsideGracePeriod => GetIsCurrentlyOutsideGracePeriod(GracePeriod, BeginDateTime);

        public static bool GetIsRunning(DateTime? actualBeginDateTime, DateTime? actualEndDateTime) => actualBeginDateTime != null && actualEndDateTime == null;

        public static bool GetIsCancelledBeforeCutoff(DateTime? cancelledDateTime, DateTime beginDateTime)
        {
            if (!cancelledDateTime.HasValue)
                return false;
            else
                return cancelledDateTime.Value.AddHours(2) < beginDateTime;
        }

        public static bool GetIsCurrentlyOutsideGracePeriod(int gracePeriod, DateTime beginDateTime)
        {
            DateTime gp = beginDateTime.AddMinutes(gracePeriod);
            return DateTime.Now > gp;
        }

        public static DateTime GetChargeBeginDateTime(DateTime beginDateTime, DateTime? actualBeginDateTime)
        {
            if (actualBeginDateTime.HasValue && actualBeginDateTime.Value < beginDateTime)
                return actualBeginDateTime.Value;
            return beginDateTime;
        }

        public static DateTime GetChargeEndDateTime(DateTime endDateTime, DateTime? actualEndDateTime)
        {
            if (actualEndDateTime.HasValue && actualEndDateTime.Value > endDateTime)
                return actualEndDateTime.Value;
            return endDateTime;
        }

        public static TimeSpan GetDuration(DateTime begin, DateTime end) => end - begin;
        public static TimeSpan GetActualDuration(DateTime? begin, DateTime? end) => (begin != null && end != null) ? end.Value - begin.Value : TimeSpan.Zero;
        public static TimeSpan GetOvertimeDuration(DateTime end, DateTime? actualEnd) => (actualEnd != null) ? TimeSpan.FromMinutes(Math.Max((actualEnd.Value - end).TotalMinutes, 0)) : TimeSpan.Zero;
    }

    public class ReservationWithInviteesItem : ReservationItem, IReservationWithInvitees
    {
        public IEnumerable<IReservationInvitee> Invitees { get; set; }
    }
}
