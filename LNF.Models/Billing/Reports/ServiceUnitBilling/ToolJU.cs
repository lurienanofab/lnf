﻿namespace LNF.Models.Billing.Reports.ServiceUnitBilling
{
    public class ToolJU : JournalUnitReport
    {
        public override BillingCategory BillingCategory
        {
            get { return BillingCategory.Tool; }
        }
    }
}
