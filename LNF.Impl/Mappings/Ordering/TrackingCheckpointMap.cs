using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    public class TrackingCheckpointMap : ClassMap<TrackingCheckpoint>
    {
        public TrackingCheckpointMap()
        {
            Schema("IOF.dbo");
            Id(x => x.TrackingCheckpointID);
            Map(x => x.CheckpointName);
            Map(x => x.Active);
        }
    }
}
