using FluentNHibernate.Mapping;
using LNF.Repository.Control;

namespace LNF.Impl.Mappings.Control
{
    public class BlockConfigMap:ClassMap<BlockConfig>
    {
        public BlockConfigMap()
        {
            Schema("sselControl.dbo");
            Id(x => x.ConfigID);
            References(x => x.Block, "BlockID");
            References(x => x.ModType, "ModTypeID");
            Map(x => x.ModPosition);
        }
    }
}
