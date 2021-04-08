using LNF.Billing;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using System;

namespace LNF.Impl.Billing
{
    public class ExternalInvoiceRepository : RepositoryBase, IExternalInvoiceRepository
    {
        protected IBillingTypeRepository BillingType { get; }

        public ExternalInvoiceRepository(ISessionManager mgr, IBillingTypeRepository billingType) : base(mgr)
        {
            BillingType = billingType;
        }

        public IExternalInvoiceManager GetManager(DateTime sd, DateTime ed, bool showRemote)
        {
            return new ExternalInvoiceManager(sd, ed, showRemote, BillingType, Session);
        }

        public IExternalInvoiceManager GetManager(int accountId, DateTime sd, DateTime ed, bool showRemote)
        {
            return new ExternalInvoiceManager(accountId, sd, ed, showRemote, BillingType, Session);
        }
    }
}
