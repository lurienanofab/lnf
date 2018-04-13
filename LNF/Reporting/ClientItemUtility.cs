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
        public static IEnumerable<ClientItem> SelectCurrentActiveClients()
        {
            var result = CreateClientItems(DA.Current.Query<ClientInfo>()
                .Where(x => x.ClientActive)
                .OrderBy(x => x.DisplayName));

            return result;
        }

        public static IEnumerable<ClientItem> SelectActiveClients(DateTime period)
        {
            var query = DA.Current.ActiveDataItemManager().FindActive(DA.Current.Query<ClientInfo>(), x => x.ClientID, period, period.AddMonths(1)).AsQueryable();
            var result = CreateClientItems(query.OrderBy(x => x.DisplayName));
            return result;
        }

        public static IEnumerable<ClientItem> SelectActiveManagers(DateTime period)
        {
            var managers = DA.Current.ActiveDataItemManager().FindActive(DA.Current.Query<ClientAccountInfo>().Where(x => x.Manager), x => x.ClientAccountID, period, period.AddMonths(1));

            var items = CreateClientItems(managers.Where(x => x.EmailRank == 1).AsQueryable());

            var comparer = new ClientItemEqualityComparer();

            return items.Distinct(comparer).OrderBy(x => x.LName).ThenBy(x => x.FName).ToList();
        }

        public static ClientItem GetManagerFor(int clientId, DateTime period)
        {
            var managers = DA.Current.ActiveDataItemManager().FindActive(DA.Current.Query<ClientAccountInfo>().Where(x => x.Manager), x => x.ClientAccountID, period, period.AddMonths(1));
            throw new NotImplementedException();
        }

        public static IEnumerable<ClientItem> CreateClientItems(IQueryable<ClientOrgInfoBase> clients)
        {
            if (clients == null) return null;

            int internalChargeTypeId = 5;

            return clients.Select(x => new ClientItem()
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

        public static ClientItem CreateClientItem(int clientId)
        {
            var query = DA.Current.Query<ClientInfo>().Where(x => x.ClientID == clientId);
            return CreateClientItems(query).FirstOrDefault();
        }
    }
}
