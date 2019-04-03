using LNF.Models.Data;
using LNF.Models.Scheduler;
using System;

namespace LNF.Repository.Scheduler
{
    public class ReservationInfo : IDataItem
    {
        // ***** Reservation **********************************************************************
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
        public virtual int Communities { get; set; }
        public virtual bool IsChecked { get; set; }
        public virtual bool IsSafetyTest { get; set; }
        public virtual int DemCitizenID { get; set; }
        public virtual string DemCitizenName { get; set; }
        public virtual int DemGenderID { get; set; }
        public virtual string DemGenderName { get; set; }
        public virtual int DemRaceID { get; set; }
        public virtual string DemRaceName { get; set; }
        public virtual int DemEthnicID { get; set; }
        public virtual string DemEthnicName { get; set; }
        public virtual int DemDisabilityID { get; set; }
        public virtual string DemDisabilityName { get; set; }
        public virtual int TechnicalInterestID { get; set; }
        public virtual string TechnicalInterestName { get; set; }
        public virtual bool ClientActive { get; set; }

        // ***** ClientOrg ************************************************************************
        public virtual int ClientOrgID { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Email { get; set; }
        public virtual bool IsManager { get; set; }
        public virtual bool IsFinManager { get; set; }
        public virtual DateTime? SubsidyStartDate { get; set; }
        public virtual DateTime? NewFacultyStartDate { get; set; }
        public virtual int ClientAddressID { get; set; }
        public virtual bool ClientOrgActive { get; set; }
        public virtual int DepartmentID { get; set; }
        public virtual string DepartmentName { get; set; }
        public virtual int RoleID { get; set; }
        public virtual string RoleName { get; set; }
        public virtual int MaxChargeTypeID { get; set; }
        public virtual string MaxChargeTypeName { get; set; }
        public virtual int EmailRank { get; set; }

        // ***** ClientAccount ********************************************************************
        public virtual int ClientAccountID { get; set; }
        public virtual bool IsDefault { get; set; }
        public virtual bool Manager { get; set; }
        public virtual bool ClientAccountActive { get; set; }

        // ***** Account **************************************************************************
        public virtual int AccountID { get; set; }
        public virtual string AccountName { get; set; }
        public virtual string ShortCode { get; set; }
        public virtual string AccountNumber { get; set; }
        public virtual int FundingSourceID { get; set; }
        public virtual string FundingSourceName { get; set; }
        public virtual int TechnicalFieldID { get; set; }
        public virtual string TechnicalFieldName { get; set; }
        public virtual int SpecialTopicID { get; set; }
        public virtual string SpecialTopicName { get; set; }
        public virtual int BillAddressID { get; set; }
        public virtual int ShipAddressID { get; set; }
        public virtual string InvoiceNumber { get; set; }
        public virtual string InvoiceLine1 { get; set; }
        public virtual string InvoiceLine2 { get; set; }
        public virtual DateTime? PoEndDate { get; set; }
        public virtual decimal? PoInitialFunds { get; set; }
        public virtual decimal? PoRemainingFunds { get; set; }
        public virtual int AccountTypeID { get; set; }
        public virtual string AccountTypeName { get; set; }
        public virtual bool AccountActive { get; set; }

        // ***** Org ******************************************************************************
        public virtual int OrgID { get; set; }
        public virtual string OrgName { get; set; }
        public virtual int DefClientAddressID { get; set; }
        public virtual int DefBillAddressID { get; set; }
        public virtual int DefShipAddressID { get; set; }
        public virtual bool NNINOrg { get; set; }
        public virtual bool PrimaryOrg { get; set; }
        public virtual int OrgTypeID { get; set; }
        public virtual string OrgTypeName { get; set; }
        public virtual int ChargeTypeID { get; set; }
        public virtual string ChargeTypeName { get; set; }
        public virtual int ChargeTypeAccountID { get; set; }
        public virtual bool OrgActive { get; set; }

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
        public virtual int CurrentReservationID { get; set; }
        public virtual int CurrentClientID { get; set; }
        public virtual int CurrentActivityID { get; set; }
        public virtual string CurrentFirstName { get; set; }
        public virtual string CurrentLastName { get; set; }
        public virtual string CurrentActivityName { get; set; }
        public virtual bool CurrentActivityEditable { get; set; }
        public virtual DateTime? CurrentBeginDateTime { get; set; }
        public virtual DateTime? CurrentEndDateTime { get; set; }
        public virtual string CurrentNotes { get; set; }

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
        public virtual bool HasState(ResourceState state) => ResourceItem.HasState(State, state);
        public virtual string ResourceDisplayName => ResourceItem.GetDisplayName(ResourceName, ResourceID);
        public virtual string Project => AccountItem.GetProject(AccountNumber);
        public virtual string NameWithShortCode => AccountItem.GetNameWithShortCode(AccountName, ShortCode);
        public virtual string FullAccountName => AccountItem.GetFullAccountName(AccountName, ShortCode, OrgName);
        public virtual bool IsRegularAccountType => AccountItem.GetIsRegularAccountType(AccountTypeID);
        public virtual string DisplayName => ClientItem.GetDisplayName(LName, FName);
    }
}
