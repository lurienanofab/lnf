using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF;
using LNF.Repository.Data;

namespace LNF.Data
{
    public class AccountChartFields
    {
        public AccountChartFields(Account acct)
        {
            this.acct = acct;
        }

        private Account acct;
        public string Number { get { return acct.Number; } }
        public string Account { get { return TryGetSubstring(0, 6); } }
        public string Fund { get { return TryGetSubstring(6, 5); } }
        public string Department { get { return TryGetSubstring(11, 6); } }
        public string Program { get { return TryGetSubstring(17, 5); } }
        public string Class { get { return TryGetSubstring(22, 5); } }
        public string Project { get { return TryGetSubstring(27, 7); } }
        public string ShortCode { get { return acct.ShortCode.Trim(); } }

        public override string ToString()
        {
            if (acct.Org.OrgType.OrgTypeID == 1)
                return string.Join("-", Account, Fund, Department, Program, Class, Project);
            else
                return Number;
        }

        private string TryGetSubstring(int start, int length)
        {
            if (Number.Length >= start + length)
                return Number.Substring(start, length);
            else
                return new string('?', length);
        }
    }
}
