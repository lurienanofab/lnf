namespace LNF.Models.Billing
{
    public class ApportionmentClientItem : IApportionmentClient
    {
        public int ClientID { get; set; }
        public string DisplayName { get; set; }
        public string Emails { get; set; }
        public int AccountCount { get; set; }
    }
}
