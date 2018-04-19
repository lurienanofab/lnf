using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public class ClientOrgManager : ManagerBase, IClientOrgManager
    {
        public ClientOrgManager(ISession session) : base(session) { }

        public ClientAccount GetDryBoxClientAccount(ClientOrg item)
        {
            IList<ClientAccount> query = Session.Query<ClientAccount>().Where(x => x.ClientOrg.ClientOrgID == item.ClientOrgID).ToList();
            ClientAccount ca = query.FirstOrDefault(x => DA.Use<IClientAccountManager>().HasDryBox(x));
            return ca;
        }

        public bool HasDryBox(ClientOrg item)
        {
            return GetDryBoxClientAccount(item) != null;
        }

        public BillingType GetBillingType(ClientOrg item)
        {
            var logs = Session.Query<ClientOrgBillingTypeLog>().Where(x => x.ClientOrg.ClientOrgID == item.ClientOrgID);

            ClientOrgBillingTypeLog active = logs.FirstOrDefault(x => x.DisableDate == null);

            if (active != null)
                return active.BillingType;

            return logs.OrderBy(x => x.DisableDate).Last().BillingType;
        }

        public ClientOrgInfo GetClientOrgInfo(ClientOrg item)
        {
            return Session.Single<ClientOrgInfo>(item.ClientOrgID);
        }

        public void Disable(ClientOrg item)
        {
            // when we disable a ClientOrg we might have to also disable the Client and/or disable physical access

            DA.Use<IActiveDataItemManager>().Disable(item); // normal disable of ClientOrg

            // first check for other active ClientOrgs, this one won't be included because it was just disabled
            bool otherActive = DA.Use<IClientManager>().ClientOrgs(item.Client).Any(x => x.Active);

            if (!otherActive)
                DA.Use<IActiveDataItemManager>().Disable(item.Client);

            // be sure to check physical access after this
        }

        public IEnumerable<ClientOrg> SelectByClientAccount(Client client, Account acct)
        {
            var clientAccounts = Session.Query<ClientAccount>().Where(x => x.Account == acct && x.ClientOrg.Client == client);
            return clientAccounts.Select(x => x.ClientOrg);
        }

        public IEnumerable<ClientOrg> SelectActiveClientOrgs(Client client, DateTime period, ActiveLogManager activeLogMgr)
        {
            var query = Session.Query<ClientOrg>().Where(x => x.Client == client);
            return activeLogMgr.Range(query, period, period.AddMonths(1));
        }

        public IEnumerable<ClientOrg> AllActiveManagers()
        {
            return Session.Query<ClientOrg>().Where(x => (x.IsManager || x.IsFinManager) && x.Active);
        }

        public IList<ClientOrg> SelectOrgManagers(int orgId = 0)
        {
            IList<ClientOrg> result = null;

            if (orgId > 0)
                result = Session.Query<ClientOrg>().Where(x => x.Org.OrgID == orgId && x.Active && (x.IsManager || x.IsFinManager)).ToList();
            else
                result = Session.Query<ClientOrg>().Where(x => x.Active && (x.IsManager || x.IsFinManager)).ToList();

            return result
                .OrderBy(x => x.Client.LName)
                .ThenBy(x => x.Client.FName)
                .ToList();
        }

        public ClientOrg GetPrimary(int clientId)
        {
            var result = Session.Query<ClientOrg>()
                .Where(x => x.Client.ClientID == clientId)
                .OrderByDescending(x => x.Active)
                .ThenByDescending(x => x.Org.PrimaryOrg)
                .ThenBy(x => x.ClientOrgID)
                .FirstOrDefault();

            return result;
        }

        public int GetMaxChargeTypeID(int clientId)
        {
            var result = Session.Query<ClientOrg>()
                .Where(x => x.Client.ClientID == clientId).Max(x => (int?)x.Org.OrgType.ChargeType.ChargeTypeID) ?? 0;

            return result;
        }

        public string AccountEmail(Client client, int accountId)
        {
            var co = SelectByClientAccount(client, Session.Single<Account>(accountId)).FirstOrDefault();

            if (co != null)
                return co.Email;
            else
                return string.Empty;
        }

        public string AccountPhone(Client client, int accountId)
        {
            var co = SelectByClientAccount(client, Session.Single<Account>(accountId)).FirstOrDefault();

            if (co != null)
                return co.Phone;
            else
                return string.Empty;
        }
    }
}
