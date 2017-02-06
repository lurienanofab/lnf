using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class ClientOrgMap : ClassMap<ClientOrg>
    {
        public ClientOrgMap()
        {
            Schema("sselData.dbo");
            Id(x => x.ClientOrgID);
            References(x => x.Client);
            References(x => x.Org);
            References(x => x.Department);
            References(x => x.Role);
            Map(x => x.ClientAddressID);
            Map(x => x.Phone);
            Map(x => x.Email);
            Map(x => x.IsManager);
            Map(x => x.IsFinManager);
            Map(x => x.SubsidyStartDate);
            Map(x => x.NewFacultyStartDate);
            Map(x => x.Active);
        }
    }
}
