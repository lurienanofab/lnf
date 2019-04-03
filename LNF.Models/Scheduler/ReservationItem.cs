using LNF.Models.Data;
using System;
using System.Collections.Generic;

namespace LNF.Models.Scheduler
{
    public class ReservationItem : ClientAccountItem, IReservation
    {
        public int ReservationID { get; set; }
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
        public string ClientBeginLName { get; set; }
        public string ClientBeginFName { get; set; }
        public int? ClientIDEnd { get; set; }
        public string ClientEndLName { get; set; }
        public string ClientEndFName { get; set; }
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
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public int BuildingID { get; set; }
        public string BuildingName { get; set; }
        public string BuildingDescription { get; set; }
        public bool BuildingIsActive { get; set; }
        public int LabID { get; set; }
        public string LabName { get; set; }
        public string LabDisplayName { get; set; }
        public string LabDescription { get; set; }
        public bool LabIsActive { get; set; }
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public string RoomDisplayName { get; set; }
        public int ProcessTechID { get; set; }
        public string ProcessTechName { get; set; }
        public string ProcessTechDescription { get; set; }
        public int ProcessTechGroupID { get; set; }
        public string ProcessTechGroupName { get; set; }
        public bool ProcessTechIsActive { get; set; }
        public string ResourceDescription { get; set; }
        public int Granularity { get; set; }
        public int ReservFence { get; set; }
        public int MinReservTime { get; set; }
        public int MaxReservTime { get; set; }
        public int MaxAlloc { get; set; }
        public int Offset { get; set; }
        public int GracePeriod { get; set; }
        public int ResourceAutoEnd { get; set; }
        public int MinCancelTime { get; set; }
        public int? UnloadTime { get; set; }
        public int? OTFSchedTime { get; set; }
        public bool AuthState { get; set; }
        public int AuthDuration { get; set; }
        public ResourceState State { get; set; }
        public string StateNotes { get; set; }
        public bool IsSchedulable { get; set; }
        public bool ResourceIsActive { get; set; }
        public string HelpdeskEmail { get; set; }
        public string WikiPageUrl { get; set; }
        public bool IsReady { get; set; }
        public int CurrentReservationID { get; set; }
        public int CurrentClientID { get; set; }
        public int CurrentActivityID { get; set; }
        public bool CurrentActivityEditable { get; set; }
        public string CurrentFirstName { get; set; }
        public string CurrentLastName { get; set; }
        public string CurrentActivityName { get; set; }
        public DateTime? CurrentBeginDateTime { get; set; }
        public DateTime? CurrentEndDateTime { get; set; }
        public string CurrentNotes { get; set; }
        public string ResourceDisplayName => ResourceItem.GetDisplayName(ResourceName, ResourceID);
        public TimeSpan GetReservedDuration() => EndDateTime - BeginDateTime;
        public TimeSpan GetActualDuration() => (ActualBeginDateTime != null && ActualEndDateTime != null) ? ActualEndDateTime.Value - ActualBeginDateTime.Value : TimeSpan.Zero;
        public TimeSpan GetChargeDuration() => ChargeEndDateTime - ChargeBeginDateTime;
        public TimeSpan GetOvertimeDuration() => (ActualEndDateTime != null) ? TimeSpan.FromMinutes(Math.Max((ActualEndDateTime.Value - EndDateTime).TotalMinutes, 0)) : TimeSpan.Zero;
        public bool IsRunning => GetIsRunning(ActualBeginDateTime, ActualEndDateTime);
        public bool HasState(ResourceState state) => ResourceItem.HasState(State, state);
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
    }

    public class ReservationItemWithInvitees : ReservationItem, IReservationWithInvitees
    {
        public IEnumerable<IReservationInvitee> Invitees { get; set; }
    }
}
