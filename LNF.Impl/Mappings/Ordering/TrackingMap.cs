using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    public class TrackingMap : ClassMap<Tracking>
    {
        public TrackingMap()
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
