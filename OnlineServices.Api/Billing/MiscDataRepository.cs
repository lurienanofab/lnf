using LNF.Billing;
using RestSharp;
using System;
using System.Data;

namespace OnlineServices.Api.Billing
{
    public class MiscDataRepository : ApiClient, IMiscDataRepository
    {
        internal MiscDataRepository(IRestClient rc) : base(rc) { }

        public DataTable ReadMiscData(DateTime period)
        {
            throw new NotImplementedException();
        }
    }
}
