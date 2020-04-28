using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class AccountTypeMap : ClassMap<AccountType>
    {
        internal AccountTypeMap()
        {
            Schema("sselData.dbo");
            Id(x => x.AccountTypeID);
            Map(x => x.AccountTypeName);
            Map(x => x.Description);
        }
    }
}
