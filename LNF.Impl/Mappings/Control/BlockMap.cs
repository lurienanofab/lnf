using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Control;

namespace LNF.Impl.Mappings.Control
{
    internal class BlockMap : ClassMap<Block>
    {
        internal BlockMap()
        {
            Schema("sselControl.dbo");
            Table("IOBlock");
            Id(x => x.BlockID);
            Map(x => x.BlockName);
            Map(x => x.IPAddress);
            Map(x => x.Description);
            Map(x => x.MACAddress, "MACaddress");
            HasMany(x => x.Points).KeyColumn("BlockID");
            HasMany(x => x.Configurations).KeyColumn("BlockID");
        }
    }
}
