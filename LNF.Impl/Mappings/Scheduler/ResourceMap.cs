using FluentNHibernate.Mapping;
using LNF.Models.Scheduler;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ResourceMap : ClassMap<Resource>
    {
        internal ResourceMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ResourceID).GeneratedBy.Assigned();
            References(x => x.ProcessTech, "ProcessTechID").NotFound.Ignore();
            References(x => x.Lab, "LabID").NotFound.Ignore();
            Map(x => x.ResourceName);
            Map(x => x.UseCost);
            Map(x => x.HourlyCost);
            Map(x => x.ReservFence);
            Map(x => x.Granularity);
            Map(x => x.Offset);
            Map(x => x.MinReservTime);
            Map(x => x.MaxReservTime);
            Map(x => x.MaxAlloc);
            Map(x => x.MinCancelTime);
            Map(x => x.GracePeriod);
            Map(x => x.AutoEnd);
            Map(x => x.AuthDuration);
            Map(x => x.AuthState);
            Map(x => x.State).CustomType<ResourceState>();
            Map(x => x.IsReady);
            Map(x => x.IsSchedulable);
            Map(x => x.Description);
            Map(x => x.IsActive);
            Map(x => x.OTFSchedTime);
            Map(x => x.IPAddress);
            Map(x => x.UnloadTime);
            Map(x => x.StateNotes);
            Map(x => x.HelpdeskEmail);
            Map(x => x.WikiPageUrl);
        }
    }
}
