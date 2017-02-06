namespace LNF.Models.Billing.Process
{
    public interface IProcessCommand
    {
        BillingCategory BillingCategory { get; set; }
        int ClientID { get; set; }
    }
}
