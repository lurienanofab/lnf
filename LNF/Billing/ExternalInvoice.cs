using System;

namespace LNF.Billing
{
    public class ExternalInvoice
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ExternalInvoiceHeader Header { get; set; }
        public ExternalInvoiceUsage Usage { get; set; }
    }
}
