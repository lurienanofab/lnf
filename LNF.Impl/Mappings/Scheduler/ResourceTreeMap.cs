﻿using FluentNHibernate.Mapping;
using LNF.Data;
using LNF.Impl.Repository.Scheduler;
using LNF.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ResourceTreeMap : ClassMap<ResourceTree>
    {
        internal ResourceTreeMap()
        {
            Schema("sselScheduler.dbo");
            Table("v_ResourceTree");
            ReadOnly();
            CompositeId()
                .KeyProperty(x => x.ResourceID)
                .KeyProperty(x => x.ClientID);
            Map(x => x.ResourceName);
            Map(x => x.ResourceDescription);
            Map(x => x.ResourceIsActive);
            Map(x => x.ProcessTechID);
            Map(x => x.ProcessTechName);
            Map(x => x.ProcessTechDescription);
            Map(x => x.ProcessTechIsActive);
            Map(x => x.ProcessTechGroupID);
            Map(x => x.ProcessTechGroupName);
            Map(x => x.LabID);
            Map(x => x.LabName);
            Map(x => x.LabDisplayName);
            Map(x => x.LabDescription);
            Map(x => x.LabIsActive);
            Map(x => x.RoomID);
            Map(x => x.RoomName);
            Map(x => x.BuildingID);
            Map(x => x.BuildingName);
            Map(x => x.BuildingDescription);
            Map(x => x.BuildingIsActive);
            Map(x => x.IsSchedulable);
            Map(x => x.HelpdeskEmail);
            Map(x => x.WikiPageUrl);
            Map(x => x.State).CustomType<ResourceState>();
            Map(x => x.StateNotes);
            Map(x => x.AuthDuration);
            Map(x => x.AuthState);
            Map(x => x.ReservFence);
            Map(x => x.MaxAlloc);
            Map(x => x.MinCancelTime);
            Map(x => x.ResourceAutoEnd);
            Map(x => x.UnloadTime);
            Map(x => x.Granularity);
            Map(x => x.Offset);
            Map(x => x.IsReady);
            Map(x => x.MinReservTime);
            Map(x => x.MaxReservTime);
            Map(x => x.GracePeriod);
            Map(x => x.OTFSchedTime);
            Map(x => x.CurrentReservationID);
            Map(x => x.CurrentClientID);
            Map(x => x.CurrentActivityID);
            Map(x => x.CurrentFirstName);
            Map(x => x.CurrentLastName);
            Map(x => x.CurrentActivityName);
            Map(x => x.CurrentActivityEditable);
            Map(x => x.CurrentBeginDateTime);
            Map(x => x.CurrentEndDateTime);
            Map(x => x.CurrentNotes);
            Map(x => x.UserName);
            Map(x => x.LName);
            Map(x => x.MName);
            Map(x => x.FName);
            Map(x => x.Privs).CustomType<ClientPrivilege>();
            Map(x => x.Communities);
            Map(x => x.ClientActive);
            Map(x => x.OrgID);
            Map(x => x.Email);
            Map(x => x.Phone);
            Map(x => x.MaxChargeTypeID);
            Map(x => x.ResourceClientID);
            Map(x => x.AuthLevel).CustomType<ClientAuthLevel>();
            Map(x => x.EveryoneAuthLevel).CustomType<ClientAuthLevel>();
            Map(x => x.EffectiveAuthLevel).CustomType<ClientAuthLevel>();
            Map(x => x.Expiration);
            Map(x => x.EmailNotify);
            Map(x => x.PracticeResEmailNotify);
            Map(x => x.ResourceClientClientID);
        }
    }
}
