using LNF.Data;
using LNF.Repository.Data;

namespace LNF.Impl.ModelFactory.Injections
{
    public class AccountInjection : ExtendedKnownSourceInjection<Account>
    {
        protected override void SetTarget(object target, Account obj)
        {
            SetTargetProperty(target, "FundingSourceName", obj, x => x.FundingSourceName());
            SetTargetProperty(target, "TechnicalFieldName", obj, x => x.TechnicalFieldName());
            SetTargetProperty(target, "SpecialTopicName", obj, x => x.SpecialTopicName());
            SetTargetProperty(target, "FullAccountName", obj, x => x.GetFullAccountName());
            SetTargetProperty(target, "NameWithShortCode", obj, x => x.GetNameWithShortCode());
            SetTargetProperty(target, "IsRegularAccountType", obj, x => x.IsRegularAccountType());
        }
    }
}
