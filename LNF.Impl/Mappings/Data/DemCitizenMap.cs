using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class DemCitizenMap : ClassMap<DemCitizen>
    {
        internal DemCitizenMap()
        {
            Schema("sselData.dbo");
            Id(x => x.DemCitizenID);
            Map(x => x.DemCitizenValue, "DemCitizen");
        }
    }
}
