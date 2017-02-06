using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Email;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Email
{
    public class RecipientMap : ClassMap<Recipient>
    {
        public RecipientMap()
        {
            Schema("Email.dbo");
            Id(x => x.RecipientID);
            References(x => x.Message);
            Map(x => x.ClientID);
            Map(x => x.AddressType);
            Map(x => x.AddressText);
            Map(x => x.AddressTimestamp);
        }
    }
}
