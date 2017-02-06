using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LNF.Data.ClientAccountMatrix
{
    public class MatrixUserAccount
    {
        public MatrixUserAccount()
        {
            ClientOrgID = 0;
            DisplayName = string.Empty;
            Email = string.Empty;
            AccountID = 0;
            ClientAccountID = 0;
            WarningMessage = string.Empty;
            Active = false;
            Key = string.Empty;
        }

        public int ClientOrgID { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public int AccountID { get; set; }
        public int ClientAccountID { get; set; }
        public string WarningMessage { get; set; }
        public bool Active { get; set; }
        public string Key { get; set; }
    }
}