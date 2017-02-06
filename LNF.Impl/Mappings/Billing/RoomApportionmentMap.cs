﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    public class RoomApportionmentMap : ClassMap<RoomApportionment>
    {
        public RoomApportionmentMap()
        {
            Schema("sselData.dbo");
            Table("RoomApportionmentInDaysMonthly");
            Id(x => x.AppID);
            Map(x => x.Period);
            References(x => x.Client);
            References(x => x.Room);
            References(x => x.Account);
            References(x => x.ChargeType);
            References(x => x.BillingType);
            References(x => x.Org);
            Map(x => x.ChargeDays);
            Map(x => x.PhysicalDays);
            Map(x => x.AccountDays);
            Map(x => x.Entries);
            Map(x => x.Hours);
            Map(x => x.IsDefault, "isDefault");
            Map(x => x.RoomRate);
            Map(x => x.EntryRate);
            Map(x => x.MonthlyRoomCharge);
            Map(x => x.RoomCharge);
            Map(x => x.EntryCharge);
            Map(x => x.SubsidyDiscount);
        }
    }
}
