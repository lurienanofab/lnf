namespace LNF.Models.Billing
{
    public interface IBillingType
    {
        int BillingTypeID { get; set; }
        string BillingTypeName { get; set; }
        bool IsActive { get; set; }
    }
}
