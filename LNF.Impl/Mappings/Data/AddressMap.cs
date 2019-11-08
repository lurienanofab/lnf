using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class AddressMap : ClassMap<Address>
    {
        internal AddressMap()
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
