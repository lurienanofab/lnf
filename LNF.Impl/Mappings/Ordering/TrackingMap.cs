using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    internal class TrackingMap : ClassMap<Tracking>
    {
        internal TrackingMap()
        {
            Schema("IOF.dbo");
            Id(x => x.TrackingID);
            References(x => x.TrackingCheckpoint, "CheckpointID");
            References(x => x.PurchaseOrder, "POID");
            References(x => x.Client);
            Map(x => x.TrackingData);
            Map(x => x.TrackingDateTime);
        }
    }
}
