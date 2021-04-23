namespace LNF.Billing
{
    public interface IMiscBillingChargeItem : IMiscBillingCharge
    {
        string LName { get; set; }
        string FName { get; set; }
        string DisplayName { get; }
        string AccountName { get; set; }
        string ShortCode { get; set; }
    }
}
