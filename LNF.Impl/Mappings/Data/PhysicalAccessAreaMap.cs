using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class PhysicalAccessAreaMap : ClassMap<PhysicalAccessArea>
    {
        internal PhysicalAccessAreaMap()
        {
            Schema("sselData.dbo");
            Table("v_Area");
            ReadOnly();
            Id(x => x.AreaID);
            Map(x => x.AreaName);
        }
    }
}
