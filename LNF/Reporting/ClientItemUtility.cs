using LNF.Data;
using LNF.Models.Reporting;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Reporting
{
    public static class ClientItemUtility
    {
        public static IEnumerable<ReportingClientItem> SelectCurrentActiveClients()
        {
            var result = CreateClientItems(DA.Current.Query<ClientInfo>()
                .Where(x => x.ClientActive)
                .OrderBy(x => x.DisplayName));

            return result;
        }

        public static IEnumerable<ReportingClientItem> SelectActiveClients(DateTime period)
        {
            var query = ServiceProvider.Current.ActiveDataItemManager.FindActive(DA.Current.Query<ClientInfo>(), x => x.ClientID, period, period.AddMonths(1), "Client").AsQueryable();
            var result = CreateClientItems(query.OrderBy(x => x.DisplayName));
            return result;
        }

        public static IEnumerable<ReportingClientItem> SelectActiveManagers(DateTime period)
        {
            var managers = ServiceProvider.Current.ActiveDataItemManager.FindActive(DA.Current.Query<ClientAccountInfo>().Where(x => x.Manager), x => x.ClientAccountID, period, period.AddMonths(1), "ClientAccount");

            var items = CreateClientItems(managers.Where(x => x.EmailRank == 1).AsQueryable());

            var comparer = new ReportingClientItemEqualityComparer();

            return items.Distinct(comparer).OrderBy(x => x.LName).ThenBy(x => x.FName).ToList();
        }

        public static ReportingClientItem GetManagerFor(int clientId, DateTime period)
        {
            var managers = ServiceProvider.Current.ActiveDataItemManager.FindActive(DA.Current.Query<ClientAccountInfo>().Where(x => x.Manager), x => x.ClientAccountID, period, period.AddMonths(1));
            throw new NotImplementedException();
        }

        public static IEnumerable<ReportingClientItem> CreateClientItems(IQueryable<ClientOrgInfoBase> clients)
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

        public static ReportingClientItem CreateClientItem(int clientId)
        {
            var query = DA.Current.Query<ClientInfo>().Where(x => x.ClientID == clientId);
            return CreateClientItems(query).FirstOrDefault();
        }
    }
}
