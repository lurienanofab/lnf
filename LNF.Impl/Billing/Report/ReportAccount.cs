namespace LNF.Impl.Billing.Report
{
    public class ReportAccount
    {
        public ReportAccount() { }

        public ReportAccount(string fullAccountText)
        {
            FullAccountText = fullAccountText;
        }

        private string _FullAccountText;
        private string _Account;
        private string _FundCode;
        private string _DeptID;
        private string _ProgramCode;
        private string _Class;
        private string _ProjectGrant;

        public string FullAccountText
        {
            get { return _FullAccountText; }
            set
            {
                _FullAccountText = value;
                Parse();
            }
        }

        public string Account { get { return _Account; } }
        public string FundCode { get { return _FundCode; } }
        public string DeptID { get { return _DeptID; } }
        public string ProgramCode { get { return _ProgramCode; } }
        public string Class { get { return _Class; } }
        public string ProjectGrant { get { return _ProjectGrant; } }

        private void Parse()
        {
            if (!string.IsNullOrEmpty(_FullAccountText))
            {
                _Account = StringPart(FullAccountText, 0, 6);
                _FundCode = StringPart(FullAccountText, 6, 5);
                _DeptID = StringPart(FullAccountText, 11, 6);
                _ProgramCode = StringPart(FullAccountText, 17, 5);
                _Class = StringPart(FullAccountText, 22, 5); //FullAccountText.Substring(22, 5);
                _ProjectGrant = StringPart(FullAccountText, 27, 7); //FullAccountText.Substring(27, 7);
            }
        }

        private string StringPart(string s, int start, int length)
        {
            if (s.Length >= start + length)
                return s.Substring(start, length);
            else if (s.Length >= start)
                return s.Substring(start);
            else
                return string.Empty;
        }
    }
}
