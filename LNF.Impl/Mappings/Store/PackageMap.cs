using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Store;

namespace LNF.Impl.Mappings.Store
{
    public class PackageMap : ClassMap<Package>
    {
        public PackageMap()
        {
            Schema("sselMAS.dbo");
            Id(x => x.PackageID);
            References(x => x.Item);
            Map(x => x.BaseQMultiplier);
            Map(x => x.Descriptor);
            Map(x => x.Active);
        }
    }
}
