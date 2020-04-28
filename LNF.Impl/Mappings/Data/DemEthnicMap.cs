using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class DemEthnicMap : ClassMap<DemEthnic>
    {
        internal DemEthnicMap()
        {
            Schema("sselData.dbo");
            Id(x => x.DemEthnicID);
            Map(x => x.DemEthnicValue, "DemEthnic");
        }
    }
}
