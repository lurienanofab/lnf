using System.Collections.Generic;

namespace LNF.Billing
{
    public interface IExternalInvoiceManager : IEnumerable<KeyValuePair<string, ExternalInvoiceUsage>>
    {
        IEnumerable<ExternalInvoice> GetInvoices(int accountId = 0);
        IEnumerable<ExternalInvoiceSummaryItem> GetSummary();
    }
}
