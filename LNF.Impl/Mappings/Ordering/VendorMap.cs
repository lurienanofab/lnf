using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    internal class VendorMap : ClassMap<Vendor>
    {
        internal VendorMap()
        {
            Schema("IOF.dbo");
            Id(x => x.VendorID);
            Map(x => x.ClientID).Not.Nullable();
            Map(x => x.VendorName).Not.Nullable();
            Map(x => x.Address1).Not.Nullable();
            Map(x => x.Address2).Nullable();
            Map(x => x.Address3).Nullable();
            Map(x => x.Contact).Nullable();
            Map(x => x.Phone).Not.Nullable();
            Map(x => x.Fax).Nullable();
            Map(x => x.URL).Nullable();
            Map(x => x.Email).Nullable();
            Map(x => x.Active).Not.Nullable();
            HasMany(x => x.Items).KeyColumn("VendorID").NotFound.Ignore();
        }
    }
}
