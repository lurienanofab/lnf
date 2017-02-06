using System;

namespace LNF.Models.Billing.Process
{
    public class UpdateClientBillingCommand
    {
        public int ClientID { get; set; }
        public DateTime Period { get; set; }
    }
}
