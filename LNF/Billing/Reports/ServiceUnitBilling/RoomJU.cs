﻿namespace LNF.Billing.Reports.ServiceUnitBilling
{
    public class RoomJU : JournalUnitReport
    {
        public override BillingCategory BillingCategory
        {
            get { return BillingCategory.Room; }
        }
    }
}
