using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class DemGenderMap : ClassMap<DemGender>
    {
        internal DemGenderMap()
        {
            Schema("sselData.dbo");
            Id(x => x.DemGenderID);
            Map(x => x.DemGenderValue, "DemGender");
        }
    }
}
