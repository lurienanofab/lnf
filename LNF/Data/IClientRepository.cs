﻿using LNF.Billing;
using System;
using System.Collections.Generic;

namespace LNF.Data
{
    public interface IClientRepository
    {
        string[] ActiveEmails(int clientId);
        string CleanMiddleName(string raw);
        IEnumerable<IClient> FindByCommunity(int flag, bool? active = true);
        IClient FindByDisplayName(string displayName);
        IEnumerable<IClient> FindByManager(int managerClientId, bool? active = true);
        IPasswordResetRequest GetPasswordResetRequest(string code);
        IEnumerable<IClient> FindByPeriod(IClient client, DateTime period, bool displayAllUsersToStaff = false);
        IEnumerable<IClient> FindByPrivilege(ClientPrivilege priv, bool? active = true);
        bool GetRequirePasswordReset(int clientId);
        IEnumerable<IClient> FindByTools(int[] resourceIds, bool? active = true);
        int GetActiveAccountCount(int clientId);
        IClientPreference GetClientPreference(int clientId, string appName);
        IEnumerable<ClientListItem> GetClients();
        IEnumerable<IClient> GetClients(int limit, int skip = 0);
        IEnumerable<IClient> GetAllClients();
        IEnumerable<IClient> GetClients(int[] ids);
        IEnumerable<IClient> GetActiveClients();
        IEnumerable<IClient> GetActiveClients(ClientPrivilege priv = 0);
        IEnumerable<IClient> GetActiveClients(DateTime sd, DateTime ed, ClientPrivilege priv = 0);
        IClient GetClient(string username);
        IClient GetClient(int clientId);
        IClient GetClient(int clientId, int rank);
        DateTime? LastReservation(int clientId);
        DateTime? LastRoomEntry(int clientId);
        IChargeType MaxChargeType(int clientId);
        IClient StoreClientInfo(ref int clientId, string lname, string fname, string mname, string username, ClientDemographics demographics, IEnumerable<IPriv> privs, IEnumerable<ICommunity> communities, int technicalInterestId, int orgId, int roleId, int deptId, string email, string phone, bool isManager, bool isFinManager, DateTime? subsidyStart, DateTime? newFacultyStart, int[] addedAddressIds, int[] deletedAddressIds, int[] clientManagerIds, int[] clientAccountIds, out string alert);
        ITechnicalField TechnicalField(int clientId);
        string TechnicalFieldName(int clientId);
        int TotalDaysInLab(int clientId, int roomId, DateTime period);
        bool Insert(IClient client);
        bool Update(IClient client);
        bool UpdatePhysicalAccess(IClient client, out string alert);
        ClientDemographics GetClientDemographics(int clientId);
        int UpdateClientDemographics(ClientDemographics value);
        IEnumerable<IClientAccount> GetClientAccounts();
        IPasswordResetRequest AddPasswordResetRequest(int clientId);
        IEnumerable<IClientAccount> GetClientAccounts(int clientId);
        IEnumerable<IClientAccount> GetClientAccounts(IClient client);
        IEnumerable<IClientAccount> GetClientAccounts(int clientId, int[] accountIds);
        
        IEnumerable<IClientAccount> GetActiveClientAccounts();
        IEnumerable<IClientAccount> GetActiveClientAccounts(int clientId);
        IEnumerable<IClientAccount> GetActiveClientAccounts(int[] clientIds);
        IEnumerable<IClientAccount> GetActiveClientAccounts(int clientId, DateTime sd, DateTime ed);
        IEnumerable<IClientAccount> GetActiveClientAccounts(string username);
        IEnumerable<IClientAccount> GetActiveClientAccounts(string username, DateTime sd, DateTime ed);
        IEnumerable<IClientAccountAssignment> GetClientAccountAssignments(int managerOrgId);
        string AccountEmail(int clientId, int accountId);
        string AccountPhone(int clientId, int accountId);
        IEnumerable<GenericListItem> AllActiveManagers();
        /// <summary>
        /// Returns technical managers only.
        /// </summary>
        IEnumerable<IClient> GetActiveManagers();
        IEnumerable<IClient> GetActiveManagers(bool includeFinancialManagers);
        void Disable(IClient client);
        IBillingType GetBillingType(int clientOrgId);
        int GetMaxChargeTypeID(int clientId);
        IClient GetPrimary(int clientId);
        IEnumerable<IClient> GetClientOrgs();
        IEnumerable<IClient> GetClientOrgs(int clientId);
        IEnumerable<IClient> GetActiveClientOrgs();
        IEnumerable<IClient> GetActiveClientOrgs(int clientId);
        IEnumerable<IClient> GetActiveClientOrgs(int clientId, DateTime sd, DateTime ed);
        IEnumerable<IClient> GetActiveClientOrgs(DateTime sd, DateTime ed);
        IEnumerable<IClient> SelectByClientAccount(int clientId, int accountId);
        IEnumerable<IClient> SelectOrgManagers(int orgId = 0);
        void InsertClientRemote(IClientRemote model, DateTime period);
        void DeleteClientRemote(int clientRemoteId, DateTime period);
        IEnumerable<IClientRemote> GetActiveClientRemotes(DateTime sd, DateTime ed);
        void SetRequirePasswordReset(int clientId, bool value);
        IEnumerable<IPriv> GetPrivs();
        IEnumerable<ICommunity> GetCommunities();
        IEnumerable<IClientManager> GetClientManagersByManager(int managerOrgId);
        IEnumerable<IClientManager> GetClientManagersByManaged(int clientOrgId);
        IStaffDirectory GetStaffDirectory(int staffDirectoryId);
        IStaffDirectory GetStaffDirectory(string userName);
        IEnumerable<IStaffDirectory> GetStaffDirectories(bool? active = true, bool? deleted = false);
        void SaveStaffDirectory(IStaffDirectory sd);
        IEnumerable<IAccount> GetActiveAccounts(int clientId);
        IEnumerable<IAccount> GetActiveAccounts(int clientId, DateTime sd, DateTime ed);
        IMessengerMessage CreateMessage(int clientId, string subject, string body, int parentId, bool disableReply, bool exclusive, bool acknowledgeRequired, bool blockAccess, int accessCutoff);
        void SendMessage(int messageId, int[] recipients);
        IEnumerable<IMessengerRecipient> GetMessages(int clientId, string folder);
        void CompletePasswordReset(int clientId, string code);
    }
}