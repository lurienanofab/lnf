﻿using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ReservationInfoMap : ClassMap<ReservationInfo>
    {
        internal ReservationInfoMap()
        {
            Schema("sselScheduler.dbo");
            Table("v_ReservationInfo");
            ReadOnly();
            Id(x => x.ReservationID);
            Map(x => x.ResourceID);
            Map(x => x.ResourceName);
            Map(x => x.ResourceDescription);
            Map(x => x.Granularity);
            Map(x => x.ReservFence);
            Map(x => x.MinReservTime);
            Map(x => x.MaxReservTime);
            Map(x => x.MaxAlloc);
            Map(x => x.Offset);
            Map(x => x.GracePeriod);
            Map(x => x.ResourceAutoEnd);
            Map(x => x.MinCancelTime);
            Map(x => x.UnloadTime);
            Map(x => x.OTFSchedTime);
            Map(x => x.IsReady);
            Map(x => x.AuthState);
            Map(x => x.AuthDuration);
            Map(x => x.State);
            Map(x => x.StateNotes);
            Map(x => x.IsSchedulable);
            Map(x => x.ResourceIsActive);
            Map(x => x.HelpdeskEmail);
            Map(x => x.WikiPageUrl);
            Map(x => x.ProcessTechID);
            Map(x => x.ProcessTechName);
            Map(x => x.ProcessTechDescription);
            Map(x => x.ProcessTechGroupID);
            Map(x => x.ProcessTechGroupName);
            Map(x => x.ProcessTechIsActive);
            Map(x => x.CurrentReservationID);
            Map(x => x.CurrentClientID);
            Map(x => x.CurrentActivityID);
            Map(x => x.CurrentActivityEditable);
            Map(x => x.CurrentFirstName);
            Map(x => x.CurrentLastName);
            Map(x => x.CurrentActivityName);
            Map(x => x.CurrentBeginDateTime);
            Map(x => x.CurrentEndDateTime);
            Map(x => x.CurrentNotes);
            Map(x => x.LabID);
            Map(x => x.LabName);
            Map(x => x.LabDisplayName);
            Map(x => x.LabDescription);
            Map(x => x.LabIsActive);
            Map(x => x.RoomID);
            Map(x => x.RoomName);
            Map(x => x.RoomDisplayName);
            Map(x => x.BuildingID);
            Map(x => x.BuildingName);
            Map(x => x.BuildingDescription);
            Map(x => x.BuildingIsActive);
            Map(x => x.ClientID);
            Map(x => x.UserName);
            Map(x => x.LName);
            Map(x => x.MName);
            Map(x => x.FName);
            Map(x => x.Privs);
            Map(x => x.Communities);
            Map(x => x.IsChecked);
            Map(x => x.IsSafetyTest);
            Map(x => x.DemCitizenID);
            Map(x => x.DemCitizenName);
            Map(x => x.DemGenderID);
            Map(x => x.DemGenderName);
            Map(x => x.DemRaceID);
            Map(x => x.DemRaceName);
            Map(x => x.DemEthnicID);
            Map(x => x.DemEthnicName);
            Map(x => x.DemDisabilityID);
            Map(x => x.DemDisabilityName);
            Map(x => x.TechnicalInterestID);
            Map(x => x.TechnicalInterestName);
            Map(x => x.ClientActive);
            Map(x => x.ClientOrgID);
            Map(x => x.Phone);
            Map(x => x.Email);
            Map(x => x.IsManager);
            Map(x => x.IsFinManager);
            Map(x => x.SubsidyStartDate);
            Map(x => x.NewFacultyStartDate);
            Map(x => x.ClientAddressID);
            Map(x => x.ClientOrgActive);
            Map(x => x.DepartmentID);
            Map(x => x.DepartmentName);
            Map(x => x.RoleID);
            Map(x => x.RoleName);
            Map(x => x.MaxChargeTypeID);
            Map(x => x.MaxChargeTypeName);
            Map(x => x.EmailRank);
            Map(x => x.ClientAccountID);
            Map(x => x.IsDefault);
            Map(x => x.Manager);
            Map(x => x.ClientAccountActive);
            Map(x => x.AccountID);
            Map(x => x.AccountName);
            Map(x => x.ShortCode);
            Map(x => x.AccountNumber);
            Map(x => x.FundingSourceID);
            Map(x => x.FundingSourceName);
            Map(x => x.TechnicalFieldID);
            Map(x => x.TechnicalFieldName);
            Map(x => x.SpecialTopicID);
            Map(x => x.SpecialTopicName);
            Map(x => x.BillAddressID);
            Map(x => x.ShipAddressID);
            Map(x => x.InvoiceNumber);
            Map(x => x.InvoiceLine1);
            Map(x => x.InvoiceLine2);
            Map(x => x.PoEndDate);
            Map(x => x.PoInitialFunds);
            Map(x => x.PoRemainingFunds);
            Map(x => x.AccountTypeID);
            Map(x => x.AccountTypeName);
            Map(x => x.AccountActive);
            Map(x => x.OrgID);
            Map(x => x.OrgName);
            Map(x => x.DefClientAddressID);
            Map(x => x.DefBillAddressID);
            Map(x => x.DefShipAddressID);
            Map(x => x.NNINOrg);
            Map(x => x.PrimaryOrg);
            Map(x => x.OrgTypeID);
            Map(x => x.OrgTypeName);
            Map(x => x.ChargeTypeID);
            Map(x => x.ChargeTypeName);
            Map(x => x.ChargeTypeAccountID);
            Map(x => x.OrgActive);
            Map(x => x.ActivityID);
            Map(x => x.ActivityName);
            Map(x => x.ActivityAccountType);
            Map(x => x.StartEndAuth);
            Map(x => x.Editable);
            Map(x => x.IsFacilityDownTime);
            Map(x => x.BeginDateTime);
            Map(x => x.EndDateTime);
            Map(x => x.ActualBeginDateTime);
            Map(x => x.ActualEndDateTime);
            Map(x => x.ChargeBeginDateTime);
            Map(x => x.ChargeEndDateTime);
            Map(x => x.ClientIDBegin);
            Map(x => x.ClientBeginLName);
            Map(x => x.ClientBeginFName);
            Map(x => x.ClientIDEnd);
            Map(x => x.ClientEndLName);
            Map(x => x.ClientEndFName);
            Map(x => x.CreatedOn);
            Map(x => x.LastModifiedOn);
            Map(x => x.Duration);
            Map(x => x.Notes);
            Map(x => x.ChargeMultiplier);
            Map(x => x.ApplyLateChargePenalty);
            Map(x => x.AutoEnd);
            Map(x => x.HasProcessInfo);
            Map(x => x.HasInvitees);
            Map(x => x.IsActive);
            Map(x => x.IsStarted);
            Map(x => x.IsUnloaded);
            Map(x => x.RecurrenceID);
            Map(x => x.GroupID);
            Map(x => x.MaxReservedDuration);
            Map(x => x.CancelledDateTime);
            Map(x => x.KeepAlive);
            Map(x => x.OriginalBeginDateTime);
            Map(x => x.OriginalEndDateTime);
            Map(x => x.OriginalModifiedOn);
        }
    }
}