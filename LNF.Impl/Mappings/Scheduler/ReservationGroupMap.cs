using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Scheduler;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Scheduler
{
    public class ReservationGroupMap : ClassMap<ReservationGroup>
    {
        public ReservationGroupMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.GroupID);
            References(x => x.Client, "ClientID");
            References(x => x.Account, "AccountID");
            References(x => x.Activity, "ActivityID");
            Map(x => x.BeginDateTime);
            Map(x => x.EndDateTime);
            Map(x => x.CreatedOn);
            Map(x => x.IsActive);
        }
    }
}
