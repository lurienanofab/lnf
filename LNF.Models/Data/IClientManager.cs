using LNF.Models.Billing;
using System;
using System.Collections.Generic;

namespace LNF.Models.Data
{
    public interface IClientManager
    {
        IEnumerable<IAccount> ActiveAccounts(int clientId);
        IEnumerable<IAccount> ActiveAccounts(int clientId, DateTime sd, DateTime ed);
        IEnumerable<IClientAccount> ActiveClientAccounts(int clientId);
        IEnumerable<IClientAccount> ActiveClientAccounts(int clientId, DateTime sd, DateTime ed);
        IEnumerable<IClient> ActiveClientOrgs(int clientId);
        IEnumerable<IClient> ActiveClientOrgs(int clientId, DateTime sd, DateTime ed);
        string[] ActiveEmails(int clientId);
        bool CheckPassword(int clientId, string password);
        string CleanMiddleName(string raw);
        IEnumerable<IClientAccount> ClientAccounts(int clientId);
        IEnumerable<IClient> ClientOrgs(int clientId);   
        IEnumerable<IClient> FindByCommunity(int flag, bool? active = true);
        IClient FindByDisplayName(string displayName);
        IEnumerable<IClient> FindByManager(int managerClientId, bool? active = true);
        IEnumerable<IClient> FindByPeriod(IClient client, DateTime period, bool displayAllUsersToStaff = false);
        IEnumerable<IClient> FindByPrivilege(ClientPrivilege priv, bool? active = true);
        IEnumerable<IClient> FindByTools(int[] resourceIds, bool? active = true);
        int GetActiveAccountCount(int clientId);
        IEnumerable<IClient> GetClients(int limit, int skip = 0);
        IEnumerable<IClient> GetActiveClients();
        IEnumerable<IClient> GetActiveClients(ClientPrivilege priv = 0);
        IEnumerable<IClient> GetActiveClients(DateTime sd, DateTime ed, ClientPrivilege priv = 0);
        IClient GetClient(string username);
        IClient GetClient(int clientId);
        IClient GetClient(int clientId, int rank);
        DateTime? LastReservation(int clientId);
        DateTime? LastRoomEntry(int clientId);
        IClient Login(string username, string password);
        IChargeType MaxChargeType(int clientId);
        int SetPassword(int clientId, string password);
        IClient StoreClientInfo(ref int clientId, string lname, string fname, string mname, string username, IClientDemographics demographics, IEnumerable<IPriv> privs, IEnumerable<ICommunity> communities, int technicalFieldId, int orgId, int roleId, int deptId, string email, string phone, bool isManager, bool isFinManager, DateTime? subsidyStart, DateTime? newFacultyStart, int[] addedAddressIds, int[] deletedAddressIds, int[] clientManagerIds, int[] clientAccountIds, out string alert);
        ITechnicalField TechnicalField(int clientId);
        string TechnicalFieldName(int clientId);
        int TotalDaysInLab(int clientId, int roomId, DateTime period);
        bool Insert(IClient client);
        bool Update(IClient client);
        bool UpdatePhysicalAccess(IClient client, out string alert);
        IClientDemographics GetClientDemographics(int clientId);
        bool UpdateClientDemographics(IClientDemographics value);
        IEnumerable<IClientAccount> GetClientAccounts(int clientId);
        IEnumerable<IClientAccount> GetActiveClientAccounts(int clientId);
        IEnumerable<IClientAccount> GetActiveClientAccounts(int clientId, DateTime sd, DateTime ed);
        string AccountEmail(int clientId, int accountId);
        string AccountPhone(int clientId, int accountId);
        IEnumerable<ListItem> AllActiveManagers();
        void Disable(IClient client);
        IBillingType GetBillingType(int clientOrgId);
        int GetMaxChargeTypeID(int clientId);
        IClient GetPrimary(int clientId);
        IEnumerable<IClient> GetClientOrgs(int clientId);
        IEnumerable<IClient> GetActiveClientOrgs(int clientId);
        IEnumerable<IClient> GetActiveClientOrgs(int clientId, DateTime sd, DateTime ed);
        IEnumerable<IClient> SelectByClientAccount(int clientId, int accountId);
        IEnumerable<IClient> SelectOrgManagers(int orgId = 0);
        void InsertClientRemote(IClientRemote model, DateTime period);
        void DeleteClientRemote(int clientRemoteId, DateTime period);
        IEnumerable<IClientRemote> GetActiveClientRemotes(DateTime sd, DateTime ed);
        IEnumerable<IPriv> GetPrivs();
        IEnumerable<ICommunity> GetCommunities();
    }
}