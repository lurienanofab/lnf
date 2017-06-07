namespace LNF.Models.Reporting
{
    public class AccountItem
    {
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string ShortCode { get; set; }
        public string Project { get; set; }
        public int OrgID { get; set; }
        public string OrgName { get; set; }
        public bool IsExternal { get { return string.IsNullOrEmpty(ShortCode.Trim()); } }
    }
}
