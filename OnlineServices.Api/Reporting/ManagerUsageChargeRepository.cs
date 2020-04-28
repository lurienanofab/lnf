using LNF.Reporting;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Reporting
{
    public class ManagerUsageChargeRepository : ApiClient, IManagerUsageChargeRepository
    {
        public IEnumerable<IManagerUsageCharge> GetManagerUsageCharges(DateTime sd, DateTime ed, bool remote = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IManagerUsageCharge> GetManagerUsageCharges(int clientId, DateTime sd, DateTime ed, bool remote = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IManagerUsageCharge> SelectByManager(int clientId, DateTime period, bool includeRemote)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IManagerUsageCharge> SelectByPeriod(DateTime period, bool includeRemote)
        {
            throw new NotImplementedException();
        }
    }
}
