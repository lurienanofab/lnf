using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Control;

namespace LNF.Impl.Mappings.Control
{
    public class PointInfoMap : ClassMap<PointInfo>
    {
        public PointInfoMap()
        {
            Schema("sselControl.dbo");
            Table("v_PointInfo");
            ReadOnly();
            Id(x => x.Index, "[Index]");
            Map(x => x.PointID);
            Map(x => x.BlockID);
            Map(x => x.BlockName);
            Map(x => x.IPAddress);
            Map(x => x.ModPosition);
            Map(x => x.Offset);
            Map(x => x.InstanceName);
            Map(x => x.ActionID);
            Map(x => x.ActionName);
        }
    }
}
