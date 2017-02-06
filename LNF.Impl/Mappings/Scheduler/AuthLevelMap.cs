using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Scheduler;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Scheduler
{
    public class AuthLevelMap : ClassMap<AuthLevel>
    {
        public AuthLevelMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.AuthLevelID).GeneratedBy.Assigned();
            Map(x => x.AuthLevelName);
            Map(x => x.Authorizable);
        }
    }
}
