using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class TechnicalFieldMap : ClassMap<TechnicalField>
    {
        internal TechnicalFieldMap()
        {
            Schema("sselData.dbo");
            Id(x => x.TechnicalFieldID);
            Map(x => x.TechnicalFieldName, "TechnicalField");
        }
    }
}
