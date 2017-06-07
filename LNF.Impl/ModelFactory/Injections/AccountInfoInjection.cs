using LNF.Repository.Data;

namespace LNF.Impl.ModelFactory.Injections
{
    public class AccountInfoInjection : ExtendedKnownSourceInjection<AccountInfo>
    {
        protected override void SetTarget(object target, AccountInfo obj)
        {
            SetTargetProperty(target, "FullAccountName", obj, x => Account.GetFullAccountName(x.ShortCode, x.AccountName, x.OrgName));
            SetTargetProperty(target, "NameWithShortCode", obj, x => Account.GetNameWithShortCode(x.ShortCode, x.AccountName));
            SetTargetProperty(target, "IsRegularAccountType", obj, x => x.AccountTypeID == 1);
        }
    }
}
