using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public interface IClientManager : IManager
    {
        IQueryable<Account> ActiveAccounts(Client client);
        IQueryable<Account> ActiveAccounts(Client client, DateTime sd, DateTime ed);
        IQueryable<ClientAccountInfo> ActiveClientAccountInfos(Client client);
        IQueryable<ClientAccount> ActiveClientAccounts(Client client);
        IQueryable<ClientAccount> ActiveClientAccounts(Client client, DateTime sd, DateTime ed);
        IQueryable<ClientOrg> ActiveClientOrgs(Client client);
        IQueryable<ClientOrg> ActiveClientOrgs(Client client, DateTime sd, DateTime ed);
        string[] ActiveEmails(Client client);
        bool CheckPassword(int clientId, string password);
        string CleanMiddleName(string raw);
        IQueryable<ClientAccount> ClientAccounts(Client client);
        IQueryable<ClientOrg> ClientOrgs(Client client);
        Client Find(string username);
        IEnumerable<Client> FindByCommunity(int flag, bool? active = true);
        Client FindByDisplayName(string displayName);
        IEnumerable<Client> FindByManager(int managerClientId, bool? active = true);
        IEnumerable<Client> FindByPeriod(Client client, DateTime period, bool displayAllUsersToStaff = false);
        IEnumerable<Client> FindByPrivilege(ClientPrivilege priv, bool? active = true);
        IEnumerable<Client> FindByTools(IEnumerable<int> resourceIds, bool? active = true);
        int GetActiveAccountCount(int clientId);
        IEnumerable<Client> GetActiveClients();
        IEnumerable<Client> GetActiveClients(ClientPrivilege priv);
        IEnumerable<Client> GetActiveClients(DateTime sd, DateTime ed);
        IQueryable<Client> GetActiveClients(DateTime sd, DateTime ed, ClientPrivilege priv);
        ClientInfo GetClientInfo(Client client);
        ClientOrgInfo GetClientOrgInfo(Client client, int rank);
        DateTime? LastReservation(Client client);
        DateTime? LastRoomEntry(Client client);
        ClientItem Login(string username, string password);
        ChargeType MaxChargeType(Client client);
        Client NewClient(string username, string password, string lname, string fname, ClientPrivilege privs, bool active);
        ClientOrg PrimaryClientOrg(Client client);
        string PrimaryEmail(Client client);
        Org PrimaryOrg(Client client);
        string PrimaryPhone(Client client);
        int SetPassword(int clientId, string password);
        Client StoreClientInfo(ref int clientId, string lname, string fname, string mname, string username, ClientDemographics demographics, IEnumerable<Priv> privs, IEnumerable<Community> communities, int technicalFieldId, Org org, Role role, Department dept, string email, string phone, bool isManager, bool isFinManager, DateTime? subsidyStart, DateTime? newFacultyStart, IEnumerable<Address> addedAddress, IEnumerable<Address> deletedAddress, IEnumerable<Repository.Data.ClientManager> clientManagers, IEnumerable<ClientAccount> clientAccounts, out string alert);
        TechnicalField TechnicalField(Client client);
        string TechnicalFieldName(Client client);
        int TotalDaysInLab(Client client, Room r, DateTime period);
        void Update(Client client, ClientItem item, ActiveDataItemManager activeDataItemMgr);
        bool UpdatePhysicalAccess(AccessCheck check, out string alert);
    }
}