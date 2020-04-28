using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class CommunityMap : ClassMap<Community>
    {
        internal CommunityMap()
        {
            Schema("sselData.dbo");
            Id(x => x.CommunityID);
            Map(x => x.CommunityFlag).ReadOnly();
            Map(x => x.CommunityName, "Community");
        }
    }
}
