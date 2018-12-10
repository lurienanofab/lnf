using LNF.Models.Billing;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Billing
{
    public class AccountSubsidyClient : ApiClient, IAccountSubsidyClient
    {
        public IEnumerable<AccountSubsidyItem> GetAccountSubsidy(DateTime sd, DateTime ed)
        {
            return Get<List<AccountSubsidyItem>>("webapi/billing/account-subsidy", QueryStrings(new { sd, ed }));
        }

        public AccountSubsidyItem AddAccountSubsidy(AccountSubsidyItem model)
        {
            return Post<AccountSubsidyItem>("webapi/billing/account-subsidy", model);
        }

        public AccountSubsidyItem DisableAccountSubsidy(int accountSubsidyId)
        {
            return Get<AccountSubsidyItem>("webapi/billing/account-subsidy/disable/{accountSubsidyId}", UrlSegments(new { accountSubsidyId }));
        }
    }
}
