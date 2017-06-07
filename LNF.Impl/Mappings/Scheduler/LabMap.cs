using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class LabMap : ClassMap<Lab>
    {
        internal LabMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.LabID);
            References(x => x.Building, "BuildingID").NotFound.Ignore();
            Map(x => x.LabName);
            Map(x => x.DisplayName);
            Map(x => x.Description);
            References(x => x.Room, "RoomID").NotFound.Ignore();
            Map(x => x.IsActive);
        }
    }
}
