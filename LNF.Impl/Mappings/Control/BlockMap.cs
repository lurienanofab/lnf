using FluentNHibernate.Mapping;
using LNF.Repository.Control;

namespace LNF.Impl.Mappings.Control
{
    public class BlockMap : ClassMap<Block>
    {
        public BlockMap()
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
