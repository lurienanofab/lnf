using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Control;

namespace LNF.Impl.Mappings.Control
{
    internal class BlockConfigMap:ClassMap<BlockConfig>
    {
        internal BlockConfigMap()
        {
            Schema("sselControl.dbo");
            Id(x => x.ConfigID);
            Map(x => x.BlockID);
            Map(x => x.ModTypeID);
            Map(x => x.ModPosition);
        }
    }
}
