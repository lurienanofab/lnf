using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class HolidayMap:ClassMap<Holiday>
    {
        internal HolidayMap()
        {
            Schema("sselData.dbo");
            Id(x => x.HolidayID);
            Map(x => x.Description, "Descript");
            Map(x => x.HolidayDate, "Holidate");
        }
    }
}
