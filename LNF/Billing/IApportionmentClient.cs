namespace LNF.Billing
{
    public interface IApportionmentClient
    {
        int ClientID { get; set; }
        string DisplayName { get; set; }
        string Emails { get; set; }
        int AccountCount { get; set; }
    }
}