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
        IClient Find(string username);
        IEnumerable<IClient> FindByCommunity(int flag, bool? active = true);
        IClient FindByDisplayName(string displayName);
        IEnumerable<IClient> FindByManager(int managerClientId, bool? active = true);
        IEnumerable<IClient> FindByPeriod(IClient client, DateTime period, bool displayAllUsersToStaff = false);
        IEnumerable<IClient> FindByPrivilege(ClientPrivilege priv, bool? active = true);
        IEnumerable<IClient> FindByTools(int[] resourceIds, bool? active = true);
        int GetActiveAccountCount(int clientId);
        IEnumerable<IClient> GetActiveClients();
        IEnumerable<IClient> GetActiveClients(ClientPrivilege priv);
        IEnumerable<IClient> GetActiveClients(DateTime sd, DateTime ed);
        IEnumerable<IClient> GetActiveClients(DateTime sd, DateTime ed, ClientPrivilege priv);
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
        void Update(IClient client);
        bool UpdatePhysicalAccess(IClient client, out string alert);
    }
}