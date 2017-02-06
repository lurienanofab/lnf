using FluentNHibernate.Mapping;
using LNF.Models.Data;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class PrivMap : ClassMap<Priv>
    {
        public PrivMap()
        {
            Schema("sselData.dbo");
            Id(x => x.PrivFlag, "Priv").GeneratedBy.Assigned().CustomType<ClientPrivilege>();
            Map(x => x.PrivType);
            Map(x => x.PrivDescription);
        }
    }
}
