using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Reporting;
using LNF.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Reporting
{
    public class ClientManagerLogRepository : RepositoryBase, IClientManagerLogRepository
    {
        public ClientManagerLogRepository(ISessionManager mgr) : base(mgr) { }

        public IEnumerable<IClientManagerLog> SelectByManager(int clientId, DateTime sd, DateTime ed)
        {
            var query = Session.Query<ClientManagerLog>().Where(x => x.ManagerClientID == clientId
                && (x.ManagerEnableDate < ed && (x.ManagerDisableDate == null || x.ManagerDisableDate.Value > sd))
                && (x.UserEnableDate < ed && (x.UserDisableDate == null || x.UserDisableDate.Value > sd)));

            var result = query.ToList();

            return result;
        }

        public IEnumerable<IClientManagerLog> SelectByPeriod(DateTime sd, DateTime ed)
        {
            var query = Session.Query<ClientManagerLog>().Where(x =>
                (x.ManagerEnableDate < ed && (x.ManagerDisableDate == null || x.ManagerDisableDate.Value > sd))
                && (x.UserEnableDate < ed && (x.UserDisableDate == null || x.UserDisableDate.Value > sd)));

            return query.ToList();
        }
    }
}
