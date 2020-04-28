using LNF.Billing;
using LNF.Data;
using LNF.Scheduler;
using System;
using System.Data;

namespace OnlineServices.Api.Billing
{
    public class ToolDataRepository : ApiClient, IToolDataRepository
    {
        public IBillingType GetBillingType(IToolData item)
        {
            throw new NotImplementedException();
        }

        public IReservation GetReservation(IToolData item)
        {
            throw new NotImplementedException();
        }

        public IRoom GetRoom(IToolData item)
        {
            throw new NotImplementedException();
        }

        public DataTable ReadToolData(DateTime period, int clientId = 0, int resourceId = 0)
        {
            throw new NotImplementedException();
        }

        public DataSet ReadToolDataClean(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0)
        {
            throw new NotImplementedException();
        }

        public DataTable ReadToolDataRaw(DateTime sd, DateTime ed, int clientId = 0)
        {
            throw new NotImplementedException();
        }

        public DataTable ReadToolUtilization(string sumCol, bool includeForgiven, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }
    }
}
