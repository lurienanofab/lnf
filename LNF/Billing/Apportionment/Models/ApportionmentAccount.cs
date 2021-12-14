namespace LNF.Billing.Apportionment.Models
{
    public class ApportionmentAccount
    {
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string ShortCode { get; set; }
        public int OrgID { get; set; }
        public string OrgName { get; set; }
        public bool Active { get; set; }
    }
}
