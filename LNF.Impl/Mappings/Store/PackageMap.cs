using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Store;

namespace LNF.Impl.Mappings.Store
{
    internal class PackageMap : ClassMap<Package>
    {
        internal PackageMap()
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
