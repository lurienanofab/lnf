using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Reporting;
using LNF.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Reporting
{
    public class ManagerUsageChargeRepository : RepositoryBase, IManagerUsageChargeRepository
    {
        public ManagerUsageChargeRepository(ISessionManager mgr) : base(mgr) { }

        public IEnumerable<IManagerUsageCharge> GetManagerUsageCharges(DateTime sd, DateTime ed, bool remote = false)
        {
            return Session.Query<ManagerUsageCharge>().Where(x => x.Period >= sd && x.Period < ed && (!x.IsRemote || remote)).ToList();
        }

        public IEnumerable<IManagerUsageCharge> GetManagerUsageCharges(int clientId, DateTime sd, DateTime ed, bool remote = false)
        {
            return Session.Query<ManagerUsageCharge>().Where(x => x.ManagerClientID == clientId && x.Period >= sd && x.Period < ed && (!x.IsRemote || remote)).ToList();
        }

        public IEnumerable<IManagerUsageCharge> SelectByManager(int clientId, DateTime period, bool includeRemote)
        {
            var query = Session.Query<ManagerUsageCharge>().Where(x => x.Period == period && x.ManagerClientID == clientId && (!x.IsRemote || includeRemote));
            return query.ToList();
        }

        public IEnumerable<IManagerUsageCharge> SelectByPeriod(DateTime period, bool includeRemote)
        {
            var query = Session.Query<ManagerUsageCharge>().Where(x => x.Period == period && (!x.IsRemote || includeRemote));
            return query.ToList();
        }
    }
}
