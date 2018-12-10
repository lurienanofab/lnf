using LNF.Models.Data;
using LNF.Models.Data.Utility.BillingChecks;
using System;
using System.Collections.Generic;

namespace LNF.Models
{
    public interface IDataService
    {
        // Clients
        IEnumerable<ClientItem> GetClients(int limit, int skip = 0);
        IEnumerable<ClientItem> GetActiveClients(ClientPrivilege privs = 0);
        IEnumerable<ClientItem> GetActiveClientsInRange(DateTime sd, DateTime ed, ClientPrivilege privs = 0);
        ClientItem GetClient(int clientId);
        ClientItem GetClient(string username);
        ClientDemographics GetClientDemographics(int clientId);
        ClientItem InsertClient(ClientItem client);
        bool UpdateClient(ClientItem client);
        IEnumerable<ClientAccountItem> GetClientAccounts(int clientId);
        IEnumerable<ClientAccountItem> GetActiveClientAccounts(int clientId);
        IEnumerable<ClientAccountItem> GetActiveClientAccountsInRange(int clientId, DateTime sd, DateTime ed);
        IEnumerable<ClientItem> GetClientOrgs(int clientId);
        IEnumerable<ClientItem> GetActiveClientOrgs(int clientId);
        IEnumerable<ClientItem> GetActiveClientOrgsInRange(int clientId, DateTime sd, DateTime ed);
        AccountItem GetAccount(int accountId);
        IEnumerable<ClientRemoteItem> GetActiveClientRemotesInRange(DateTime sd, DateTime ed);
        ClientRemoteItem InsertClientRemote(ClientRemoteItem model, DateTime period);
        int DeleteClientRemote(int clientRemoteId);

        // Costs
        IEnumerable<CostItem> GetCosts(int limit, int skip = 0);
        CostItem GetCost(int costId);
        IEnumerable<CostItem> GetResourceCosts(int resourceId, DateTime? cutoff = null, int? chargeTypeId = null);

        IEnumerable<DryBoxItem> GetDryBoxes();
        bool UpdateDryBox(DryBoxItem model);
        IEnumerable<ServiceLogItem> GetServiceLogs(Guid? id = null, string service = null, string subject = null);
        ServiceLogItem InsertServiceLog(ServiceLogItem model);
        bool UpdateServiceLog(Guid id, string data);
        IEnumerable<AutoEndProblem> GetAutoEndProblems(DateTime period);
        int FixAllAutoEndProblems(DateTime period);
        int FixAutoEndProblem(DateTime period, int reservationId);
        string GetSiteMenu(int clientId);
    }
}
