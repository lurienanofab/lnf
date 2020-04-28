using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Control;

namespace LNF.Impl.Mappings.Control
{
    internal class PointMap : ClassMap<Point>
    {
        internal PointMap()
        {
            Schema("sselControl.dbo");
            Id(x => x.PointID);
            References(x => x.Block, "BlockID");
            Map(x => x.ModPosition);
            Map(x => x.Offset);
            Map(x => x.Name);
        }
    }
}
