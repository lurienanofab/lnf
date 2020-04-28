using System;

namespace LNF.Billing
{
    public class UpdateClientBillingCommand
    {
        public int ClientID { get; set; }
        public DateTime Period { get; set; }
    }
}
