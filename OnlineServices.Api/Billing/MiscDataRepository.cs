using LNF.Billing;
using System;
using System.Data;

namespace OnlineServices.Api.Billing
{
    public class MiscDataRepository : ApiClient, IMiscDataRepository
    {
        public DataTable ReadMiscData(DateTime period)
        {
            throw new NotImplementedException();
        }
    }
}
