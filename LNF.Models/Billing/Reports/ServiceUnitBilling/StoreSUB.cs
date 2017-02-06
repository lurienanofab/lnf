namespace LNF.Models.Billing.Reports.ServiceUnitBilling
{
    public class StoreSUB : ServiceUnitBillingReport
    {
        public bool TwoCreditAccounts { get; set; }

        public override BillingCategory BillingCategory
        {
            get { return BillingCategory.Store; }
        }
    }
}
