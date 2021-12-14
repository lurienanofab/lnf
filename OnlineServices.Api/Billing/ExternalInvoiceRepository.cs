using LNF.Billing;
using RestSharp;
using System;

namespace OnlineServices.Api.Billing
{
    public class ExternalInvoiceRepository : ApiClient, IExternalInvoiceRepository
    {
        internal ExternalInvoiceRepository(IRestClient rc) : base(rc) { }

        public IExternalInvoiceManager GetManager(DateTime sd, DateTime ed, bool showRemote)
        {
            throw new NotImplementedException();
        }

        public IExternalInvoiceManager GetManager(int accountId, DateTime sd, DateTime ed, bool showRemote)
        {
            throw new NotImplementedException();
        }
    }
}
