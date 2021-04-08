using FluentNHibernate.Mapping;
using LNF.Data;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ReservationInviteeInfoMap : ClassMap<ReservationInviteeInfo>
    {
        internal ReservationInviteeInfoMap()
        {
            Schema("sselScheduler.dbo");
            Table("v_ReservationInviteeInfo");
            ReadOnly();
            CompositeId()
                .KeyProperty(x => x.ReservationID)
                .KeyProperty(x => x.InviteeID);
            Map(x => x.ResourceID);
            Map(x => x.ResourceName);
            Map(x => x.ResourceDescription);
            Map(x => x.ResourceIsActive);
            Map(x => x.ResourceAutoEnd);
            Map(x => x.MinCancelTime);
            Map(x => x.UnloadTime);
            Map(x => x.OTFSchedTime);
            Map(x => x.IsReady);
            Map(x => x.MinReservTime);
            Map(x => x.MaxReservTime);
            Map(x => x.MaxAlloc);
            Map(x => x.ReservFence);
            Map(x => x.Granularity);
            Map(x => x.Offset);
            Map(x => x.GracePeriod);
            Map(x => x.AuthState);
            Map(x => x.AuthDuration);
            Map(x => x.State);
            Map(x => x.StateNotes);
            Map(x => x.IsSchedulable);
            Map(x => x.HelpdeskEmail);
            Map(x => x.WikiPageUrl);
            Map(x => x.ProcessTechID);
            Map(x => x.ProcessTechName);
            Map(x => x.ProcessTechDescription);
            Map(x => x.ProcessTechGroupID);
            Map(x => x.ProcessTechGroupName);
            Map(x => x.ProcessTechIsActive);
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
            Map(x => x.ClientOrgID);
            Map(x => x.ClientAddressID);
            Map(x => x.Phone);
            Map(x => x.Email);
            Map(x => x.IsManager);
            Map(x => x.IsFinManager);
            Map(x => x.SubsidyStartDate);
            Map(x => x.NewFacultyStartDate);
            Map(x => x.ClientOrgActive);
            Map(x => x.AccountID);
            Map(x => x.AccountName);
            Map(x => x.ShortCode);
            Map(x => x.ChargeTypeID);
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
            Map(x => x.ReservationAutoEnd);
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
            Map(x => x.InviteeActive);
            Map(x => x.InviteeLName);
            Map(x => x.InviteeFName);
            Map(x => x.InviteePrivs).CustomType<ClientPrivilege>();
        }
    }
}
