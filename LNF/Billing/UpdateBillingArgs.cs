using System;

namespace LNF.Billing
{
    public class UpdateBillingArgs
    {
        public int ClientID { get; set; }
        public DateTime[] Periods { get; set; }
        public BillingCategory BillingCategory { get; set; }
    }
}