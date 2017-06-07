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
            var result = DA.Current.Query<ClientInfo>()
                .Where(x => x.ClientActive)
                .OrderBy(x => x.DisplayName)
                .Select(CreateClientItem);

            return result.ToList();
        }

        public static IEnumerable<ClientItem> SelectActiveClients(DateTime period)
        {
            var result = DA.Current.Query<ClientInfo>()
                .FindActive(x => x.ClientID, period, period.AddMonths(1))
                .OrderBy(x => x.DisplayName)
                .Select(CreateClientItem);

            return result.ToList();
        }

        public static IEnumerable<ClientItem> SelectActiveManagers(DateTime period)
        {
            var managers = DA.Current.Query<ClientAccountInfo>().Where(x => x.Manager).FindActive(x => x.ClientAccountID, period, period.AddMonths(1));

            var items = managers.Where(x => x.EmailRank == 1).Select(CreateClientItem).ToList();

            var comparer = new ClientItemEqualityComparer();
            return items.Distinct(comparer).OrderBy(x => x.LName).ThenBy(x => x.FName).ToList();
        }

        public static ClientItem GetManagerFor(int clientId, DateTime period)
        {
            var managers = DA.Current.Query<ClientAccountInfo>().Where(x => x.Manager).FindActive(x => x.ClientAccountID, period, period.AddMonths(1));

            throw new NotImplementedException();
        }

        public static ClientItem CreateClientItem(ClientOrgInfoBase client)
        {
            if (client == null) return null;

            return new ClientItem()
            {
                ClientID = client.ClientID,
                UserName = client.UserName,
                LName = client.LName,
                FName = client.FName,
                Email = client.Email
            };
        }

        public static ClientItem CreateClientItem(int clientId)
        {
            ClientInfo c = DA.Current.Query<ClientInfo>().FirstOrDefault(x => x.ClientID == clientId);
            return CreateClientItem(c);
        }
    }
}
