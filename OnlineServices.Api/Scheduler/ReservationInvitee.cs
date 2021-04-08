using LNF.Data;
using LNF.Scheduler;
using System;

namespace OnlineServices.Api.Scheduler
{
    public class ReservationInvitee : IReservationInvitee
    {
        public bool Active { get; set; }
        public DateTime? ActualBeginDateTime { get; set; }
        public DateTime? ActualEndDateTime { get; set; }
        public DateTime BeginDateTime { get; set; }
        public string DisplayName => Clients.GetDisplayName(LName, FName);
        public DateTime EndDateTime { get; set; }
        public string FName { get; set; }
        public int InviteeID { get; set; }
        public bool IsActive { get; set; }
        public bool IsStarted { get; set; }
        public string LName { get; set; }
        public ClientPrivilege Privs { get; set; }
        public int ClientOrgID { get; set; }
        public int ClientAddressID { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsManager { get; set; }
        public bool IsFinManager { get; set; }
        public DateTime? SubsidyStartDate { get; set; }
        public DateTime? NewFacultyStartDate { get; set; }
        public bool ClientOrgActive { get; set; }
        public bool Removed { get; set; }
        public int ReservationID { get; set; }
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public bool InviteeActive { get; set; }
        public string InviteeLName { get; set; }
        public string InviteeFName { get; set; }
        public string InviteeDisplayName => Clients.GetDisplayName(InviteeLName, InviteeFName);
        public ClientPrivilege InviteePrivs { get; set; }
        public string MName { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string ShortCode { get; set; }
        public int ChargeTypeID { get; set; }
        public int ActivityID { get; set; }
        public string ActivityName { get; set; }
        public ActivityAccountType ActivityAccountType { get; set; }
        public ClientAuthLevel StartEndAuth { get; set; }
        public bool Editable { get; set; }

        public bool IsRepair => throw new NotImplementedException();

        public bool IsFacilityDownTime { get; set; }

        public DateTime ChargeBeginDateTime => throw new NotImplementedException();

        public DateTime ChargeEndDateTime => throw new NotImplementedException();

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
        public bool ReservationAutoEnd { get; set; }
        public bool HasProcessInfo { get; set; }
        public bool HasInvitees { get; set; }
        public bool IsUnloaded { get; set; }
        public int? RecurrenceID { get; set; }
        public int? GroupID { get; set; }
        public double MaxReservedDuration { get; set; }
        public DateTime? CancelledDateTime { get; set; }
        public bool KeepAlive { get; set; }
        public DateTime? OriginalBeginDateTime { get; set; }
        public DateTime? OriginalEndDateTime { get; set; }
        public DateTime? OriginalModifiedOn { get; set; }
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public string RoomDisplayName { get; set; }

        public bool IsRunning => throw new NotImplementedException();

        public bool IsCancelledBeforeCutoff => throw new NotImplementedException();

        public bool IsCurrentlyOutsideGracePeriod => throw new NotImplementedException();

        public bool ResourceIsActive { get; set; }
        public bool IsSchedulable { get; set; }
        public string ResourceDescription { get; set; }
        public string HelpdeskEmail { get; set; }
        public string WikiPageUrl { get; set; }
        public ResourceState State { get; set; }
        public string StateNotes { get; set; }
        public int AuthDuration { get; set; }
        public bool AuthState { get; set; }
        public int ReservFence { get; set; }
        public int MaxAlloc { get; set; }
        public int MinCancelTime { get; set; }
        public int ResourceAutoEnd { get; set; }
        public int? UnloadTime { get; set; }
        public int? OTFSchedTime { get; set; }
        public int Granularity { get; set; }
        public int Offset { get; set; }
        public bool IsReady { get; set; }
        public int MinReservTime { get; set; }
        public int MaxReservTime { get; set; }
        public int GracePeriod { get; set; }

        public string ResourceDisplayName => throw new NotImplementedException();

        public int ProcessTechID { get; set; }
        public int ProcessTechGroupID { get; set; }
        public string ProcessTechGroupName { get; set; }
        public string ProcessTechDescription { get; set; }
        public bool ProcessTechIsActive { get; set; }
        public string ProcessTechName { get; set; }
        public int LabID { get; set; }
        public string LabName { get; set; }
        public string LabDescription { get; set; }
        public string LabDisplayName { get; set; }
        public bool LabIsActive { get; set; }
        public string BuildingDescription { get; set; }
        public int BuildingID { get; set; }
        public bool BuildingIsActive { get; set; }
        public string BuildingName { get; set; }
        public int ClientID { get; set; }
        public string UserName { get; set; }

        public TimeSpan GetActualDuration()
        {
            throw new NotImplementedException();
        }

        public TimeSpan GetChargeDuration()
        {
            throw new NotImplementedException();
        }

        public DateTime GetNextGranularity(DateTime now, GranularityDirection dir)
        {
            throw new NotImplementedException();
        }

        public TimeSpan GetOvertimeDuration()
        {
            throw new NotImplementedException();
        }

        public TimeSpan GetReservedDuration()
        {
            throw new NotImplementedException();
        }

        public string GetResourceName(ResourceNamePartial part)
        {
            throw new NotImplementedException();
        }

        public bool HasState(ResourceState state)
        {
            throw new NotImplementedException();
        }
    }
}
