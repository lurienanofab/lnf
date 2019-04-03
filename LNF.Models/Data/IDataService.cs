using LNF.Models.Data;
using LNF.Models.Data.Utility.BillingChecks;
using System;
using System.Collections.Generic;

namespace LNF.Models.Data
{
    public interface IDataService
    {
        IClientManager ClientManager { get; }
        IAccountManager AccountManager { get; }
        IChargeTypeManager ChargeTypeManager { get; }
        IRoomManager RoomManager { get; }

        // Clients
        IEnumerable<IClient> GetClients(int limit, int skip = 0);
        IEnumerable<IClient> GetActiveClients(ClientPrivilege privs = 0);
        IEnumerable<IClient> GetActiveClientsInRange(DateTime sd, DateTime ed, ClientPrivilege privs = 0);
        IClient GetClient(int clientId);
        IClient GetClient(string username);
        IClientDemographics GetClientDemographics(int clientId);
        bool InsertClient(IClient client);
        bool UpdateClient(IClient client);
        IEnumerable<IClientAccount> GetClientAccounts(int clientId);
        IEnumerable<IClientAccount> GetActiveClientAccounts(int clientId);
        IEnumerable<IClientAccount> GetActiveClientAccountsInRange(int clientId, DateTime sd, DateTime ed);
        IEnumerable<IClient> GetClientOrgs(int clientId);
        IEnumerable<IClient> GetActiveClientOrgs(int clientId);
        IEnumerable<IClient> GetActiveClientOrgsInRange(int clientId, DateTime sd, DateTime ed);
        IAccount GetAccount(int accountId);
        IEnumerable<IClientRemote> GetActiveClientRemotesInRange(DateTime sd, DateTime ed);
        bool InsertClientRemote(IClientRemote model, DateTime period);
        int DeleteClientRemote(int clientRemoteId);

        // Costs
        IEnumerable<ICost> GetCosts(int limit, int skip = 0);
        ICost GetCost(int costId);
        IEnumerable<ICost> GetResourceCosts(int resourceId, DateTime? cutoff = null, int? chargeTypeId = null);

        IEnumerable<IDryBox> GetDryBoxes();
        bool UpdateDryBox(IDryBox model);

        IEnumerable<IServiceLog> GetServiceLogs(Guid? id = null, string service = null, string subject = null);
        bool InsertServiceLog(IServiceLog model);
        bool UpdateServiceLog(Guid id, string data);

        IEnumerable<AutoEndProblem> GetAutoEndProblems(DateTime period);
        int FixAllAutoEndProblems(DateTime period);
        int FixAutoEndProblem(DateTime period, int reservationId);

        string GetSiteMenu(int clientId);
    }
}
