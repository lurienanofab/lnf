using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class HolidayMap:ClassMap<Holiday>
    {
        public HolidayMap()
        {
            Schema("sselData.dbo");
            Id(x => x.HolidayID);
            Map(x => x.Description, "Descript");
            Map(x => x.HolidayDate, "Holidate");
        }
    }
}
