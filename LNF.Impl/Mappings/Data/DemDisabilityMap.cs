using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class DemDisabilityMap : ClassMap<DemDisability>
    {
        internal DemDisabilityMap()
        {
            Schema("sselData.dbo");
            Id(x => x.DemDisabilityID);
            Map(x => x.DemDisabilityValue, "DemDisability");
        }
    }
}
