using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class StaffDirectoryMap : ClassMap<StaffDirectory>
    {
        internal StaffDirectoryMap()
        {
            Schema("sselData.dbo");
            Id(x => x.StaffDirectoryID);
            References(x => x.Client);
            Map(x => x.HoursXML);
            Map(x => x.ContactPhone);
            Map(x => x.Office);
            Map(x => x.Deleted);
            Map(x => x.LastUpdate);
        }
    }
}
