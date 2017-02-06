namespace LNF.Models.Billing.Reports.ServiceUnitBilling
{
    public class ToolSUB : ServiceUnitBillingReport
    {
        public override BillingCategory BillingCategory
        {
            get { return BillingCategory.Tool; }
        }
    }
}
