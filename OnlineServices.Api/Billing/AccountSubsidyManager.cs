using LNF.Models.Billing;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Billing
{
    public class AccountSubsidyManager : ApiClient, IAccountSubsidyManager
    {
        public IEnumerable<IAccountSubsidy> GetAccountSubsidy(int? accountId = null)
        {
            return Get<List<AccountSubsidyItem>>("webapi/billing/account-subsidy", QueryStrings(new { accountId }));
        }

        public IEnumerable<IAccountSubsidy> GetActiveAccountSubsidy(DateTime sd, DateTime ed)
        {
            return Get<List<AccountSubsidyItem>>("webapi/billing/account-subsidy/active", QueryStrings(new { sd, ed }));
        }

        public int AddAccountSubsidy(IAccountSubsidy model)
        {
            return Post<int>("webapi/billing/account-subsidy", model);
        }

        public bool DisableAccountSubsidy(int accountSubsidyId)
        {
            return Get<bool>("webapi/billing/account-subsidy/disable/{accountSubsidyId}", UrlSegments(new { accountSubsidyId }));
        }
    }
}
