using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    public class RoomDataImportMap:ClassMap<RoomDataImport>
    {
        public RoomDataImportMap()
        {
            Schema("Billing.dbo");
            Id(x => x.RoomDataImportID);
            Map(x => x.RID).Unique();
            Map(x => x.ClientID);
            Map(x => x.RoomName);
            Map(x => x.EventDate);
            Map(x => x.EventDescription);
        }
    }
}
