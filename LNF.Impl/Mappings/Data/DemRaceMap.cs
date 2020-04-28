using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class DemRaceMap : ClassMap<DemRace>
    {
        internal DemRaceMap()
        {
            Schema("sselData.dbo");
            Id(x => x.DemRaceID);
            Map(x => x.DemRaceValue, "DemRace");
        }
    }
}
