using LNF.Data;
using LNF.DataAccess;
using LNF.Scheduler;
using System;

namespace LNF.Impl.Repository.Scheduler
{
    public class ReservationInviteeInfo : ReservationInviteeBase, IReservationInvitee, IDataItem
    {
        public virtual bool IsUnloaded { get; set; }
        public virtual double MaxReservedDuration { get; set; }
        public virtual DateTime? OriginalBeginDateTime { get; set; }
        public virtual DateTime? OriginalEndDateTime { get; set; }
        public virtual DateTime? OriginalModifiedOn { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual string ResourceDisplayName => Resources.GetResourceDisplayName(ResourceName, ResourceID);
        public virtual int ReservFence { get; set; }
        public virtual int Granularity { get; set; }
        public virtual int Offset { get; set; }
        public virtual int MinReservTime { get; set; }
        public virtual int MaxReservTime { get; set; }
        public virtual int MinCancelTime { get; set; }
        public virtual int GracePeriod { get; set; }
        public virtual int ResourceAutoEnd { get; set; }
        public virtual int AuthDuration { get; set; }
        public virtual bool AuthState { get; set; }
        public virtual int LabID { get; set; }
        public virtual int BuildingID { get; set; }
        public virtual int ActivityID { get; set; }
        public virtual string ActivityName { get; set; }
        public virtual bool Editable { get; set; }
        public virtual bool IsRepair => !Editable;
        public virtual ActivityAccountType ActivityAccountType { get; set; }
        public virtual ClientAuthLevel StartEndAuth { get; set; }
        public virtual bool IsFacilityDownTime { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string UserName { get; set; }
        public virtual ClientPrivilege Privs { get; set; }
        public virtual string LName { get; set; }
        public virtual string MName { get; set; }
        public virtual string FName { get; set; }
        public virtual string DisplayName => Clients.GetDisplayName(LName, FName);
        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }
        public virtual int AccountID { get; set; }
        public virtual string AccountName { get; set; }
        public virtual string ShortCode { get; set; }
        public virtual int ChargeTypeID { get; set; }
        public virtual DateTime ChargeBeginDateTime => Reservations.GetChargeBeginDateTime(BeginDateTime, ActualBeginDateTime);
        public virtual DateTime ChargeEndDateTime => Reservations.GetChargeEndDateTime(EndDateTime, ActualEndDateTime);
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
        public virtual bool ReservationAutoEnd { get; set; }
        public virtual bool HasProcessInfo { get; set; }
        public virtual bool HasInvitees { get; set; }
        public virtual int? RecurrenceID { get; set; }
        public virtual int? GroupID { get; set; }
        public virtual DateTime? CancelledDateTime { get; set; }
        public virtual bool KeepAlive { get; set; }
        public virtual bool IsCancelledBeforeCutoff => Reservations.GetIsCancelledBeforeCutoff(CancelledDateTime, BeginDateTime);
        public virtual bool IsCurrentlyOutsideGracePeriod => Reservations.GetIsCurrentlyOutsideGracePeriod(GracePeriod, BeginDateTime);
        public virtual bool IsRunning => Reservations.GetIsRunning(ActualBeginDateTime, ActualEndDateTime);
        public virtual bool ResourceIsActive { get; set; }
        public virtual bool IsSchedulable { get; set; }
        public virtual string ResourceDescription { get; set; }
        public virtual string HelpdeskEmail { get; set; }
        public virtual string WikiPageUrl { get; set; }
        public virtual ResourceState State { get; set; }
        public virtual string StateNotes { get; set; }
        public virtual int MaxAlloc { get; set; }
        public virtual int? UnloadTime { get; set; }
        public virtual int? OTFSchedTime { get; set; }
        public virtual bool IsReady { get; set; }
        public virtual int ProcessTechGroupID { get; set; }
        public virtual string ProcessTechGroupName { get; set; }
        public virtual string ProcessTechDescription { get; set; }
        public virtual bool ProcessTechIsActive { get; set; }
        public virtual string ProcessTechName { get; set; }
        public virtual string LabName { get; set; }
        public virtual string LabDescription { get; set; }
        public virtual string LabDisplayName { get; set; }
        public virtual bool LabIsActive { get; set; }
        public virtual string BuildingDescription { get; set; }
        public virtual bool BuildingIsActive { get; set; }
        public virtual string BuildingName { get; set; }
        public virtual int RoomID { get; set; }
        public virtual string RoomName { get; set; }
        public virtual string RoomDisplayName { get; set; }
        public virtual int ClientOrgID { get; set; }
        public virtual int ClientAddressID { get; set; }
        public virtual bool IsManager { get; set; }
        public virtual bool IsFinManager { get; set; }
        public virtual DateTime? SubsidyStartDate { get; set; }
        public virtual DateTime? NewFacultyStartDate { get; set; }
        public virtual bool ClientOrgActive { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is ReservationInviteeInfo item)) return false;
            return item.ReservationID == ReservationID && item.InviteeID == InviteeID;
        }

        public override int GetHashCode()
        {
            return ReservationID.GetHashCode() * 17 + InviteeID.GetHashCode();
        }

        public virtual TimeSpan GetActualDuration() => Reservations.GetActualDuration(ActualBeginDateTime, ActualEndDateTime);

        public virtual TimeSpan GetChargeDuration() => Reservations.GetDuration(ChargeBeginDateTime, ChargeEndDateTime);

        public virtual DateTime GetNextGranularity(DateTime now, GranularityDirection dir) => Resources.GetNextGranularity(Granularity, Offset, now, dir);

        public virtual TimeSpan GetOvertimeDuration() => Reservations.GetOvertimeDuration(EndDateTime, ActualEndDateTime);

        public virtual TimeSpan GetReservedDuration() => Reservations.GetDuration(BeginDateTime, EndDateTime);

        public virtual string GetResourceName(ResourceNamePartial part) => Resources.GetResourceName(this, part);

        public virtual bool HasState(ResourceState state) => Resources.HasState(State, state);
    }
}
