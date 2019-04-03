using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ResourceInfoMap : ClassMap<ResourceInfo>
    {
        internal ResourceInfoMap()
        {
            Schema("sselScheduler.dbo");
            Table("v_ResourceInfo");
            ReadOnly();
            Id(x => x.ResourceID);
            Map(x => x.ProcessTechID);
            Map(x => x.LabID);
            Map(x => x.BuildingID);
            Map(x => x.ResourceName);
            Map(x => x.ProcessTechName);
            Map(x => x.ProcessTechDescription);
            Map(x => x.ProcessTechGroupID);
            Map(x => x.ProcessTechGroupName);
            Map(x => x.ProcessTechIsActive);
            Map(x => x.LabName);
            Map(x => x.LabDisplayName);
            Map(x => x.LabDescription);
            Map(x => x.LabIsActive);
            Map(x => x.RoomID);
            Map(x => x.RoomName);
            Map(x => x.RoomDisplayName);
            Map(x => x.BuildingName);
            Map(x => x.BuildingDescription);
            Map(x => x.BuildingIsActive);
            Map(x => x.ResourceIsActive, "IsActive");
            Map(x => x.IsSchedulable);
            Map(x => x.ResourceDescription, "Description");
            Map(x => x.HelpdeskEmail);
            Map(x => x.WikiPageUrl);
            Map(x => x.State);
            Map(x => x.StateNotes);
            Map(x => x.AuthDuration);
            Map(x => x.AuthState);
            Map(x => x.ReservFence);
            Map(x => x.MaxAlloc);
            Map(x => x.MinCancelTime);
            Map(x => x.ResourceAutoEnd, "AutoEnd");
            Map(x => x.UnloadTime);
            Map(x => x.OTFSchedTime);
            Map(x => x.Granularity);
            Map(x => x.Offset);
            Map(x => x.IsReady);
            Map(x => x.MinReservTime);
            Map(x => x.MaxReservTime);
            Map(x => x.GracePeriod);
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
        }
    }
}
