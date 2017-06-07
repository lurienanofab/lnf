using System.Collections.Generic;

namespace LNF.Models.Reporting
{
    public class AccountItemEqualityComparer : IEqualityComparer<AccountItem>
    {
        public bool Equals(AccountItem x, AccountItem y)
        {
            return x.AccountID == y.AccountID;
        }

        public int GetHashCode(AccountItem obj)
        {
            return obj.AccountID.GetHashCode();
        }
    }
}
