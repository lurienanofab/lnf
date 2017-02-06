using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Ordering;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Ordering
{
    public class StatusMap : ClassMap<Status>
    {
        public StatusMap()
        {
            Schema("IOF.dbo");
            Id(x => x.StatusID);
            Map(x => x.StatusName, "Status");
        }
    }
}
