using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Impl.ModelFactory.Injections
{
    public class AccountInjection : ExtendedKnownSourceInjection<Account>
    {
        protected override void SetTarget(object target, Account obj)
        {
            var mgr = DA.Current.AccountManager();
            SetTargetProperty(target, "FundingSourceName", obj, x => mgr.FundingSourceName(x));
            SetTargetProperty(target, "TechnicalFieldName", obj, x => mgr.TechnicalFieldName(x));
            SetTargetProperty(target, "SpecialTopicName", obj, x => mgr.SpecialTopicName(x));
            SetTargetProperty(target, "FullAccountName", obj, x => x.GetFullAccountName());
            SetTargetProperty(target, "NameWithShortCode", obj, x => x.GetNameWithShortCode());
            SetTargetProperty(target, "IsRegularAccountType", obj, x => x.IsRegularAccountType());
        }
    }
}
