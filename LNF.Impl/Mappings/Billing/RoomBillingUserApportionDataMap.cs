using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    public class RoomBillingUserApportionDataMap : ClassMap<RoomBillingUserApportionData>
    {
        public RoomBillingUserApportionDataMap()
        {
            Id(x => x.RoomBillingUserApportionDataID, "AppID");
            Map(x => x.Period);
            References(x => x.Client);
            References(x => x.Room);
            References(x => x.Account);
            Map(x => x.ChargeDays);
            Map(x => x.Entries);
        }
    }
}
