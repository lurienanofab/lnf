using FluentNHibernate.Mapping;
using LNF.Repository.Control;

namespace LNF.Impl.Mappings.Control
{
    public class PointMap : ClassMap<Point>
    {
        public PointMap()
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
