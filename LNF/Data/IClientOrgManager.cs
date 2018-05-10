using System;
using System.Collections.Generic;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;

namespace LNF.Data
{
    public interface IClientOrgManager : IManager
    {
        string AccountEmail(Client client, int accountId);
        string AccountPhone(Client client, int accountId);
        IEnumerable<ClientOrg> AllActiveManagers();
        void Disable(ClientOrg item);
        BillingType GetBillingType(ClientOrg item);
        ClientOrgInfo GetClientOrgInfo(ClientOrg item);
        int GetMaxChargeTypeID(int clientId);
        ClientOrg GetPrimary(int clientId);
        IEnumerable<ClientOrg> SelectActiveClientOrgs(Client client, DateTime period, ActiveLogManager activeLogMgr);
        IEnumerable<ClientOrg> SelectByClientAccount(Client client, Account acct);
        IList<ClientOrg> SelectOrgManagers(int orgId = 0);
    }
}