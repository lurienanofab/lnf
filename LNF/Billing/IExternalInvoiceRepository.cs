using System;

namespace LNF.Billing
{
    public interface IExternalInvoiceRepository
    {
        IExternalInvoiceManager GetManager(DateTime sd, DateTime ed, bool showRemote);
        IExternalInvoiceManager GetManager(int accountId, DateTime sd, DateTime ed, bool showRemote);
    }
}
