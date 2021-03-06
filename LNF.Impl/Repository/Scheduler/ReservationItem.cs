﻿using LNF.Data;
using LNF.DataAccess;
using LNF.Scheduler;
using System;

namespace LNF.Impl.Repository.Scheduler
{
    public class ReservationItem : IReservationItem, IDataItem
    {
        public virtual int ReservationID { get; set; }
        public virtual int ResourceID { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual string ResourceDisplayName => Resources.GetResourceDisplayName(ResourceName, ResourceID);
        public virtual int Granularity { get; set; }
        public virtual int Offset { get; set; }
        public virtual int MinReservTime { get; set; }
        public virtual int MaxReservTime { get; set; }
        public virtual int MinCancelTime { get; set; }
        public virtual int GracePeriod { get; set; }
        public virtual int ReservFence { get; set; }
        public virtual int AuthDuration { get; set; }
        public virtual bool AuthState { get; set; }
        public virtual int ResourceAutoEnd { get; set; }
        public virtual int ProcessTechID { get; set; }
        public virtual string ProcessTechName { get; set; }
        public virtual int LabID { get; set; }
        public virtual int BuildingID { get; set; }
        public virtual int ActivityID { get; set; }
        public virtual string ActivityName { get; set; }
        public virtual bool Editable { get; set; }
        public virtual bool IsRepair => !Editable;
        public virtual bool IsFacilityDownTime { get; set; }
        public virtual ActivityAccountType ActivityAccountType { get; set; }
        public virtual ClientAuthLevel StartEndAuth { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string UserName { get; set; }
        public virtual ClientPrivilege Privs { get; set; }
        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }
        public virtual string LName { get; set; }
        public virtual string MName { get; set; }
        public virtual string FName { get; set; }
        public virtual string DisplayName => Clients.GetDisplayName(LName, FName);
        public virtual int AccountID { get; set; }
        public virtual string AccountName { get; set; }
        public virtual string ShortCode { get; set; }
        public virtual int ChargeTypeID { get; set; }
        public virtual int? RecurrenceID { get; set; }
        public virtual int? GroupID { get; set; }
        public virtual int? ClientIDBegin { get; set; }
        public virtual int? ClientIDEnd { get; set; }
        public virtual DateTime BeginDateTime { get; set; }
        public virtual DateTime EndDateTime { get; set; }
        public virtual DateTime? ActualBeginDateTime { get; set; }
        public virtual DateTime? ActualEndDateTime { get; set; }
        public virtual DateTime ChargeBeginDateTime => Reservations.GetChargeBeginDateTime(BeginDateTime, ActualBeginDateTime);
        public virtual DateTime ChargeEndDateTime => Reservations.GetChargeEndDateTime(EndDateTime, ActualEndDateTime);
        public virtual double Duration { get; set; }
        public virtual double ChargeMultiplier { get; set; }
        public virtual bool ApplyLateChargePenalty { get; set; }
        public virtual bool ReservationAutoEnd { get; set; }
        public virtual bool HasProcessInfo { get; set; }
        public virtual bool HasInvitees { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool IsStarted { get; set; }
        public virtual bool KeepAlive { get; set; }
        public virtual string Notes { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual DateTime LastModifiedOn { get; set; }
        public virtual DateTime? CancelledDateTime { get; set; }
        public virtual TimeSpan GetReservedDuration() => Reservations.GetDuration(BeginDateTime, EndDateTime);
        public virtual TimeSpan GetActualDuration() => Reservations.GetActualDuration(ActualBeginDateTime, ActualEndDateTime);
        public virtual TimeSpan GetChargeDuration() => Reservations.GetDuration(ChargeBeginDateTime, ChargeEndDateTime);
        public virtual TimeSpan GetOvertimeDuration() => Reservations.GetOvertimeDuration(EndDateTime, ActualEndDateTime);
        public virtual bool IsCancelledBeforeCutoff => Reservations.GetIsCancelledBeforeCutoff(CancelledDateTime, BeginDateTime);
        public virtual bool IsCurrentlyOutsideGracePeriod => Reservations.GetIsCurrentlyOutsideGracePeriod(GracePeriod, BeginDateTime);
        public virtual bool IsRunning => Reservations.GetIsRunning(ActualBeginDateTime, ActualEndDateTime);
        public virtual DateTime GetNextGranularity(DateTime now, GranularityDirection dir) => Resources.GetNextGranularity(Granularity, Offset, now, dir);
    }
}
