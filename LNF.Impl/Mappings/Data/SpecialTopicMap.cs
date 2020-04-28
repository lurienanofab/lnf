using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class SpecialTopicMap : ClassMap<SpecialTopic>
    {
        internal SpecialTopicMap()
        {
            Schema("sselData.dbo");
            Id(x => x.SpecialTopicID);
            Map(x => x.SpecialTopicName, "SpecialTopic");
        }
    }
}
