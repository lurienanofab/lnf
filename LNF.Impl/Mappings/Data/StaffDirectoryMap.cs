using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Data
{
    public class StaffDirectoryMap : ClassMap<StaffDirectory>
    {
        public StaffDirectoryMap()
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
