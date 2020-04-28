using FluentNHibernate.Mapping;
using LNF.Data;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class PrivMap : ClassMap<Priv>
    {
        internal PrivMap()
        {
            Schema("sselData.dbo");
            Id(x => x.PrivFlag, "Priv").GeneratedBy.Assigned().CustomType<ClientPrivilege>();
            Map(x => x.PrivType);
            Map(x => x.PrivDescription);
        }
    }
}
