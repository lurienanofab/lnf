using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    internal class TrackingCheckpointMap : ClassMap<TrackingCheckpoint>
    {
        internal TrackingCheckpointMap()
        {
            Schema("IOF.dbo");
            Id(x => x.TrackingCheckpointID);
            Map(x => x.CheckpointName);
            Map(x => x.Active);
        }
    }
}
