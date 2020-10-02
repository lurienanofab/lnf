using LNF.Impl.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Data;
using LNF.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Reporting
{
    public class ClientItemRepository : RepositoryBase, IClientItemRepository
    {
        public ClientItemRepository(ISessionManager mgr) : base(mgr) { }

        public IEnumerable<IReportingClient> SelectCurrentActiveClients()
        {
            var result = CreateClientItems(Session.Query<ClientInfo>()
                .Where(x => x.ClientActive)
                .OrderBy(x => x.DisplayName));

            return result;
        }

        public IEnumerable<IReportingClient> SelectActiveClients(DateTime period)
        {
            var query = Session.Query<ClientInfo>().FindActive(x => x.ClientID, period, period.AddMonths(1), ActiveLogQuery("Client")).AsQueryable();
            var result = CreateClientItems(query.OrderBy(x => x.DisplayName));
            return result;
        }

        public IEnumerable<IReportingClient> SelectActiveManagers(DateTime period)
        {
            var managers = Session.Query<ClientAccountInfo>().Where(x => x.Manager).FindActive(x => x.ClientAccountID, period, period.AddMonths(1), ActiveLogQuery("ClientAccount")).ToList();

            var items = CreateClientItems(managers.Where(x => x.EmailRank == 1).AsQueryable());

            var comparer = new ReportingClientItemEqualityComparer();

            return items.Distinct(comparer).OrderBy(x => x.LName).ThenBy(x => x.FName).ToList();
        }

        public IReportingClient GetManagerFor(int clientId, DateTime period)
        {
            var managers = Session.Query<ClientAccountInfo>().Where(x => x.Manager).FindActive(x => x.ClientAccountID, period, period.AddMonths(1), ActiveLogQuery("ClientAccount"));
            var result = CreateClientItems(managers.AsQueryable()).FirstOrDefault();
            return result;
        }

        public IReportingClient CreateClientItem(int clientId)
        {
            var query = Session.Query<ClientInfo>().Where(x => x.ClientID == clientId);
            return CreateClientItems(query).FirstOrDefault();
        }

        private IEnumerable<IReportingClient> CreateClientItems(IQueryable<ClientOrgInfoBase> clients)
        {
            if (clients == null) return null;

            int internalChargeTypeId = 5;

            return clients.Select(x => new ReportingClientItem()
            {
                ClientID = x.ClientID,
                UserName = x.UserName,
                LName = x.LName,
                FName = x.FName,
                Email = x.Email,
                IsManager = x.IsManager,
                IsFinManager = x.IsFinManager,
                IsInternal = x.ChargeTypeID == internalChargeTypeId
            }).ToList();
        }
    }
}
