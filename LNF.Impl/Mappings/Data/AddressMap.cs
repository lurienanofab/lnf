using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Data
{
    public class AddressMap : ClassMap<Address>
    {
        public AddressMap()
        {
            Schema("sselData.dbo");
            Id(x => x.AddressID);
            Map(x => x.InternalAddress);
            Map(x => x.StrAddress1);
            Map(x => x.StrAddress2);
            Map(x => x.City);
            Map(x => x.State);
            Map(x => x.Zip);
            Map(x => x.Country);
        }
    }
}
