﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    public class PurchaseOrderAccountMap : ClassMap<PurchaseOrderAccount>
    {
        public PurchaseOrderAccountMap()
        {
            Schema("IOF.dbo");
            Table("Account");
            CompositeId()
                .KeyProperty(x => x.AccountID)
                .KeyProperty(x => x.ClientID);
            Map(x => x.Active);
        }
    }
}
