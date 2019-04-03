namespace LNF.Models.Billing
{
    public interface IApportionmentClient
    {
        int AccountCount { get; set; }
        int ClientID { get; set; }
        string DisplayName { get; set; }
        string Emails { get; set; }
    }
}