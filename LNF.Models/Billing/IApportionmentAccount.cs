namespace LNF.Models.Billing
{
    public interface IApportionmentAccount
    {
        int AccountID { get; set; }
        string AccountName { get; set; }
        double AccountDays { get; set; }
        double ChargeDays { get; set; }
    }
}
