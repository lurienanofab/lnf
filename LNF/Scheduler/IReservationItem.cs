﻿using LNF.Data;
using System;
using System.Collections.Generic;

namespace LNF.Scheduler
{
    public interface IReservationItem
    {
        int ReservationID { get; set; }
        int ResourceID { get; set; }
        string ResourceName { get; set; }
        string ResourceDisplayName { get; }
        int ReservFence { get; set; }
        int Granularity { get; set; }
        int Offset { get; set; }
        int MinReservTime { get; set; }
        int MaxReservTime { get; set; }
        int MinCancelTime { get; set; }
        int GracePeriod { get; set; }
        int ResourceAutoEnd { get; set; }
        int AuthDuration { get; set; }
        bool AuthState { get; set; }
        int ProcessTechID { get; set; }
        int LabID { get; set; }
        int BuildingID { get; set; }
        int ActivityID { get; set; }
        string ActivityName { get; set; }
        bool Editable { get; set; }
        bool IsRepair { get; }
        ActivityAccountType ActivityAccountType { get; set; }
        ClientAuthLevel StartEndAuth { get; set; }
        bool IsFacilityDownTime { get; set; }
        int ClientID { get; set; }
        string UserName { get; set; }
        ClientPrivilege Privs { get; set; }
        string LName { get; set; }
        string MName { get; set; }
        string FName { get; set; }
        string DisplayName { get; }
        string Email { get; set; }
        string Phone { get; set; }
        int AccountID { get; set; }
        string AccountName { get; set; }
        string ShortCode { get; set; }
        int ChargeTypeID { get; set; }
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
        bool ReservationAutoEnd { get; set; }
        bool HasProcessInfo { get; set; }
        bool HasInvitees { get; set; }
        bool IsActive { get; set; }
        bool IsStarted { get; set; }
        int? RecurrenceID { get; set; }
        int? GroupID { get; set; }
        DateTime? CancelledDateTime { get; set; }
        bool KeepAlive { get; set; }
        bool IsCancelledBeforeCutoff { get; }
        bool IsCurrentlyOutsideGracePeriod { get; }
        bool IsRunning { get; }
        TimeSpan GetReservedDuration();
        TimeSpan GetActualDuration();
        TimeSpan GetChargeDuration();
        TimeSpan GetOvertimeDuration();
        DateTime GetNextGranularity(DateTime now, GranularityDirection dir);
    }

    public interface IReservationWithInviteesItem : IReservationItem
    {
        IEnumerable<IReservationInviteeItem> Invitees { get; set; }
    }
}
