using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Control;

namespace LNF.Impl.Mappings.Control
{
    internal class PointInfoMap : ClassMap<PointInfo>
    {
        internal PointInfoMap()
        {
            Schema("sselControl.dbo");
            Table("v_PointInfo");
            ReadOnly();
            Map(x => x.Index, "[Index]");
            Map(x => x.BlockID);
            Map(x => x.BlockName);
            Map(x => x.IPAddress);
            Id(x => x.PointID);
            Map(x => x.ModPosition);
            Map(x => x.Offset);
            Map(x => x.InstanceName);
            Map(x => x.ActionID);
            Map(x => x.ActionName);
        }
    }
}
