using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF;
using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;

namespace LNF.Data
{
    public static class ClientOrgUtility
    {
        public static IEnumerable<ClientOrg> SelectByClientAccount(Client client, Account acct)
        {
            var clientAccounts = DA.Current.Query<ClientAccount>().Where(x => x.Account == acct && x.ClientOrg.Client == client);
            return clientAccounts.Select(x => x.ClientOrg);
        }

        public static IEnumerable<ClientOrg> SelectActiveClientOrgs(Client client, DateTime period)
        {
            var query = DA.Current.Query<ClientOrg>().Where(x => x.Client == client);
            return ActiveLogUtility.Range(query, period, period.AddMonths(1));
        }

        public static IEnumerable<ClientOrg> AllActiveManagers()
        {
            return DA.Current.Query<ClientOrg>().Where(x => (x.IsManager || x.IsFinManager) && x.Active);
        }

        public static IList<ClientOrg> SelectOrgManagers(int OrgID = 0)
        {
            IList<ClientOrg> result = null;

            if (OrgID > 0)
                result = DA.Current.Query<ClientOrg>().Where(x => x.Org.OrgID == OrgID && x.Active && (x.IsManager || x.IsFinManager)).ToList();
            else
                result = DA.Current.Query<ClientOrg>().Where(x => x.Active && (x.IsManager || x.IsFinManager)).ToList();

            return result
                .OrderBy(x => x.Client.LName)
                .ThenBy(x => x.Client.FName)
                .ToList();
        }

        public static ClientOrg GetPrimary(int clientId)
        {
            var result = DA.Current.Query<ClientOrg>()
                .Where(x => x.Client.ClientID == clientId)
                .OrderByDescending(x => x.Active)
                .ThenByDescending(x => x.Org.PrimaryOrg)
                .ThenBy(x => x.ClientOrgID)
                .FirstOrDefault();

            return result;
        }

        public static int GetMaxChargeTypeID(int clientId)
        {
            var result = DA.Current.Query<ClientOrg>()
                .Where(x => x.Client.ClientID == clientId).Max(x => (int?)x.Org.OrgType.ChargeType.ChargeTypeID) ?? 0;

            return result;
        }
    }
}
