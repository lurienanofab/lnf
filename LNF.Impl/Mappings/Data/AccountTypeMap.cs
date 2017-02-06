using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Data
{
    public class AccountTypeMap : ClassMap<AccountType>
    {
        public AccountTypeMap()
        {
            Schema("sselData.dbo");
            Id(x => x.AccountTypeID);
            Map(x => x.AccountTypeName);
            Map(x => x.Description);
        }
    }
}
