using LNF.Repository.Data;

namespace LNF.Data
{
    public struct AccountNumber
    {
        private string _Value;

        public string Value { get { return _Value; } }
        public string Account { get { return TryGetSubstring(0, 6); } }
        public string Fund { get { return TryGetSubstring(6, 5); } }
        public string Department { get { return TryGetSubstring(11, 6); } }
        public string Program { get { return TryGetSubstring(17, 5); } }
        public string Class { get { return TryGetSubstring(22, 5); } }
        public string Project { get { return TryGetSubstring(27, 7); } }

        private AccountNumber(string value)
        {
            _Value = value;
        }

        public static AccountNumber Parse(string value)
        {
            return new AccountNumber(value);
        }

        private string TryGetSubstring(int start, int length)
        {
            if (_Value.Length >= start + length)
                return _Value.Substring(start, length);
            else
                return new string('?', length);
        }
    }

    public class AccountChartFields
    {
        public AccountChartFields(Account acct)
        {
            _acct = acct;
            _acctNum = AccountNumber.Parse(_acct.Number);
        }

        public static AccountChartFields Create(Account acct)
        {
            return new AccountChartFields(acct);
        }

        private Account _acct;
        private AccountNumber _acctNum;

        public int AccountID { get { return _acct.AccountID; } }
        public string AccountName { get { return _acct.Name; } }
        public string Number { get { return _acct.Number; } }
        public string Account { get { return _acctNum.Account; } }
        public string Fund { get { return _acctNum.Fund; } }
        public string Department { get { return _acctNum.Department; } }
        public string Program { get { return _acctNum.Program; } }
        public string Class { get { return _acctNum.Class; } }
        public string Project { get { return _acctNum.Project; } }
        public string ShortCode { get { return _acct.ShortCode.Trim(); } }

        public override string ToString()
        {
            if (IsChartFieldOrg(_acct.Org))
                return string.Join("-", Account, Fund, Department, Program, Class, Project);
            else
                return Number;
        }

        public static bool IsChartFieldOrg(Org org)
        {
            return org.OrgType.OrgTypeID == 1;
        }
    }
}
