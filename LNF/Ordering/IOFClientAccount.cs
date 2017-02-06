using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;

namespace LNF.Ordering
{
    public class IOFClientAccount
    {
        public int AccountID { get; set; }
        public int ClientAccountID { get; set; }
        public string AccountName { get; set; }
        public string ShortCode { get; set; }
        public string OrgName { get; set; }

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(ShortCode))
                    return AccountName;
                else
                    return string.Format("{0}, {1}", ShortCode, AccountName);
            }
        }

        public string CombinedName
        {
            get { return string.Format("{0} ({1})", AccountName, OrgName); }
        }

        public IOFClientAccount() { }

        public IOFClientAccount(ClientAccountInfo ca)
        {
            AccountID = ca.AccountID;
            ClientAccountID = ca.ClientAccountID;
            AccountName = ca.AccountName;
            ShortCode = ca.ShortCode;
            OrgName = ca.OrgName;
        }
    }
}
