using FluentNHibernate.Mapping;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    public class AccountSubsidyMap:ClassMap<AccountSubsidy>
    {
        public AccountSubsidyMap()
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
