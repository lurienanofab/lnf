using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;

namespace LNF.Ordering
{
    public class IOFAccount
    {
        public int ClientID { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string ShortCode { get; set; }

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(ShortCode))
                    return AccountName;
                else
                    return ShortCode + ": " + AccountName;
            }
        }

        public IOFAccount() { }

        public IOFAccount(ClientAccountInfo ca)
        {
            ClientID = ca.ClientID;
            AccountID = ca.AccountID;
            AccountName = ca.AccountName;
            ShortCode = ca.ShortCode;
        }
    }
}
