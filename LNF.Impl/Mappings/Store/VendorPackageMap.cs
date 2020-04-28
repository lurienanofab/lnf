using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Store;

namespace LNF.Impl.Mappings.Store
{
    internal class VendorPackageMap : ClassMap<VendorPackage>
    {
        internal VendorPackageMap()
        {
            Schema("sselMAS.dbo");
            Id(x => x.VendorPackageID);
            References(x => x.Vendor);
            References(x => x.Package);
            Map(x => x.VendorPN);
            Map(x => x.Active);
        }
    }
}
