namespace LNF.Billing.Reports.ServiceUnitBilling
{
    public class RoomSUB : ServiceUnitBillingReport
    {
        public override BillingCategory BillingCategory
        {
            get { return BillingCategory.Room; }
        }
    }
}
