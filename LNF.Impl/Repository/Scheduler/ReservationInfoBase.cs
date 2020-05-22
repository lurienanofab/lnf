﻿using LNF.Data;
using LNF.DataAccess;
using LNF.Scheduler;
using System;

namespace LNF.Impl.Repository.Scheduler
{
    public abstract class ReservationInfoBase : IReservation, IDataItem
    {
        public virtual int ReservationID { get; set; }
        public virtual DateTime BeginDateTime { get; set; }
        public virtual DateTime EndDateTime { get; set; }
        public virtual DateTime? ActualBeginDateTime { get; set; }
        public virtual DateTime? ActualEndDateTime { get; set; }
        public virtual DateTime ChargeBeginDateTime { get; set; }
        public virtual DateTime ChargeEndDateTime { get; set; }
        public virtual int? ClientIDBegin { get; set; }
        public virtual string ClientBeginLName { get; set; }
        public virtual string ClientBeginFName { get; set; }
        public virtual int? ClientIDEnd { get; set; }
        public virtual string ClientEndLName { get; set; }
        public virtual string ClientEndFName { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual DateTime LastModifiedOn { get; set; }
        public virtual double Duration { get; set; }
        public virtual string Notes { get; set; }
        public virtual double ChargeMultiplier { get; set; }
        public virtual bool ApplyLateChargePenalty { get; set; }
        public virtual bool AutoEnd { get; set; }
        public virtual bool HasProcessInfo { get; set; }
        public virtual bool HasInvitees { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool IsStarted { get; set; }
        public virtual bool IsUnloaded { get; set; }
        public virtual int? RecurrenceID { get; set; }
        public virtual int? GroupID { get; set; }
        public virtual double MaxReservedDuration { get; set; }
        public virtual DateTime? CancelledDateTime { get; set; }
        public virtual bool KeepAlive { get; set; }
        public virtual DateTime? OriginalBeginDateTime { get; set; }
        public virtual DateTime? OriginalEndDateTime { get; set; }
        public virtual DateTime? OriginalModifiedOn { get; set; }


        // ***** Client ***************************************************************************
        public virtual int ClientID { get; set; }
        public virtual string UserName { get; set; }
        public virtual string LName { get; set; }
        public virtual string MName { get; set; }
        public virtual string FName { get; set; }
        public virtual ClientPrivilege Privs { get; set; }

        // ***** ClientOrg ************************************************************************
        public virtual string Phone { get; set; }
        public virtual string Email { get; set; }

        // ***** Account **************************************************************************
        public virtual int AccountID { get; set; }
        public virtual string AccountName { get; set; }
        public virtual string ShortCode { get; set; }

        // ***** OrgType **************************************************************************
        public virtual int ChargeTypeID { get; set; }

        // ***** Resource *************************************************************************
        public virtual int ResourceID { get; set; }
        public virtual int ProcessTechID { get; set; }
        public virtual int LabID { get; set; }
        public virtual int BuildingID { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual string ProcessTechName { get; set; }
        public virtual string ProcessTechDescription { get; set; }
        public virtual int ProcessTechGroupID { get; set; }
        public virtual string ProcessTechGroupName { get; set; }
        public virtual bool ProcessTechIsActive { get; set; }
        public virtual string LabName { get; set; }
        public virtual string LabDisplayName { get; set; }
        public virtual string LabDescription { get; set; }
        public virtual bool LabIsActive { get; set; }
        public virtual int RoomID { get; set; }
        public virtual string RoomName { get; set; }
        public virtual string RoomDisplayName { get; set; }
        public virtual string BuildingName { get; set; }
        public virtual string BuildingDescription { get; set; }
        public virtual bool BuildingIsActive { get; set; }
        public virtual bool ResourceIsActive { get; set; }
        public virtual bool IsSchedulable { get; set; }
        public virtual string ResourceDescription { get; set; }
        public virtual string HelpdeskEmail { get; set; }
        public virtual string WikiPageUrl { get; set; }
        public virtual ResourceState State { get; set; }
        public virtual string StateNotes { get; set; }
        public virtual int AuthDuration { get; set; }
        public virtual bool AuthState { get; set; }
        public virtual int ReservFence { get; set; }
        public virtual int MaxAlloc { get; set; }
        public virtual int MinCancelTime { get; set; }
        public virtual int ResourceAutoEnd { get; set; }
        public virtual int? UnloadTime { get; set; }
        public virtual int? OTFSchedTime { get; set; }
        public virtual int Granularity { get; set; }
        public virtual int Offset { get; set; }
        public virtual bool IsReady { get; set; }
        public virtual int MinReservTime { get; set; }
        public virtual int MaxReservTime { get; set; }
        public virtual int GracePeriod { get; set; }

        // ***** Activity *************************************************************************
        public virtual int ActivityID { get; set; }
        public virtual string ActivityName { get; set; }
        public virtual ActivityAccountType ActivityAccountType { get; set; }
        public virtual ClientAuthLevel StartEndAuth { get; set; }
        public virtual bool Editable { get; set; }
        public virtual bool IsRepair => !Editable;
        public virtual bool IsFacilityDownTime { get; set; }

        public virtual bool IsRunning => ReservationItem.GetIsRunning(ActualBeginDateTime, ActualEndDateTime);
        public virtual bool IsCurrentlyOutsideGracePeriod => ReservationItem.GetIsCurrentlyOutsideGracePeriod(GracePeriod, BeginDateTime);
        public virtual bool IsCancelledBeforeCutoff => ReservationItem.GetIsCancelledBeforeCutoff(CancelledDateTime, BeginDateTime);
        public virtual bool HasState(ResourceState state) => Resources.HasState(State, state);

        public virtual TimeSpan GetReservedDuration() => ReservationItem.GetDuration(BeginDateTime, EndDateTime);
        public virtual TimeSpan GetActualDuration() => ReservationItem.GetActualDuration(ActualBeginDateTime, ActualEndDateTime);
        public virtual TimeSpan GetChargeDuration() => ReservationItem.GetDuration(ChargeBeginDateTime, ChargeEndDateTime);
        public virtual TimeSpan GetOvertimeDuration() => ReservationItem.GetOvertimeDuration(EndDateTime, ActualEndDateTime);

        public virtual string ResourceDisplayName => Resources.GetResourceDisplayName(ResourceName, ResourceID);
        public virtual string NameWithShortCode => Accounts.GetNameWithShortCode(AccountName, ShortCode);
        public virtual string DisplayName => Clients.GetDisplayName(LName, FName);

        public virtual string GetResourceName(ResourceNamePartial part) => Resources.GetResourceName(this, part);

        public virtual DateTime GetNextGranularity(DateTime now, GranularityDirection dir) => Resources.GetNextGranularity(Granularity, Offset, now, dir);
    }
}