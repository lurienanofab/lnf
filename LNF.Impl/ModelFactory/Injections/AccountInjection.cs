using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Impl.ModelFactory.Injections
{
    public class AccountInjection : ExtendedKnownSourceInjection<Account>
    {
        protected IAccountManager AccountManager => ServiceProvider.Current.Use<IAccountManager>();

        protected override void SetTarget(object target, Account obj)
        {
            SetTargetProperty(target, "FundingSourceName", obj, x => AccountManager.FundingSourceName(x));
            SetTargetProperty(target, "TechnicalFieldName", obj, x => AccountManager.TechnicalFieldName(x));
            SetTargetProperty(target, "SpecialTopicName", obj, x => AccountManager.SpecialTopicName(x));
            SetTargetProperty(target, "FullAccountName", obj, x => x.GetFullAccountName());
            SetTargetProperty(target, "NameWithShortCode", obj, x => x.GetNameWithShortCode());
            SetTargetProperty(target, "IsRegularAccountType", obj, x => x.IsRegularAccountType());
        }
    }
}
