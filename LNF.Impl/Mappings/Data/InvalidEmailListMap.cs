using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class InvalidEmailListMap : ClassMap<InvalidEmailList>
    {
        internal InvalidEmailListMap()
        {
            Schema("sselData.dbo");
            Id(x => x.EmailID);
            Map(x => x.DisplayName);
            Map(x => x.EmailAddress);
            Map(x => x.IsActive);
        }
    }
}
