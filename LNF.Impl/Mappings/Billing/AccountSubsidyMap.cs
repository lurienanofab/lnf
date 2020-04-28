using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class AccountSubsidyMap:ClassMap<AccountSubsidy>
    {
        internal AccountSubsidyMap()
        {
            Schema("Billing.dbo");
            Id(x => x.AccountSubsidyID);
            Map(x => x.AccountID);
            Map(x => x.UserPaymentPercentage);
            Map(x => x.CreatedDate);
            Map(x => x.EnableDate);
            Map(x => x.DisableDate);
        }
    }
}
