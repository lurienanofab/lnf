﻿using LNF;
using LNF.Billing;
using LNF.Data;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Data
{
    public class ClientRepository : ApiClient, IClientRepository
    {
        internal ClientRepository(IRestClient rc) : base(rc) { }

        public static ClientRepository Create(IRestClient rc) => new ClientRepository(rc);

        public string[] ActiveEmails(int clientId)
        {
            throw new NotImplementedException();
        }

        public bool CheckPassword(int clientId, string password)
        {
            throw new NotImplementedException();
        }

        public string CleanMiddleName(string raw)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> FindByCommunity(int flag, bool? active = true)
        {
            throw new NotImplementedException();
        }

        public IClient FindByDisplayName(string displayName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> FindByManager(int managerClientId, bool? active = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> FindByPeriod(IClient client, DateTime period, bool displayAllUsersToStaff = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> FindByPrivilege(ClientPrivilege priv, bool? active = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> FindByTools(int[] resourceIds, bool? active = true)
        {
            throw new NotImplementedException();
        }

        public int GetActiveAccountCount(int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> GetClients(int limit, int skip = 0)
        {
            return Get<List<ClientItem>>("webapi/data/client", QueryStrings(new { limit, skip }));
        }

        public IEnumerable<IClient> GetAllClients()
        {
            return Get<List<ClientItem>>("webapi/data/client/all");
        }

        public IEnumerable<IClient> GetActiveClients()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> GetActiveClients(ClientPrivilege privs = 0)
        {
            return Get<List<ClientItem>>("webapi/data/client/active", QueryStrings(new { privs = (int)privs }));
        }

        public IEnumerable<IClient> GetActiveClients(DateTime sd, DateTime ed, ClientPrivilege privs = 0)
        {
            return Get<List<ClientItem>>("webapi/data/client/active/range", QueryStrings(new { sd = sd.ToString("yyyy-MM-dd"), ed = ed.ToString("yyyy-MM-dd"), privs = (int)privs }));
        }

        public IClient GetClient(string username)
        {
            return Get<ClientItem>("webapi/data/client/username/{username}", UrlSegments(new { username }));
        }

        public IClient GetClient(int clientId)
        {
            return Get<ClientItem>("webapi/data/client/id/{clientId}", UrlSegments(new { clientId }));
        }

        public IClient GetClient(int clientId, int rank)
        {
            throw new NotImplementedException();
        }

        public DateTime? LastReservation(int clientId)
        {
            throw new NotImplementedException();
        }

        public DateTime? LastRoomEntry(int clientId)
        {
            throw new NotImplementedException();
        }

        public IClient Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public IChargeType MaxChargeType(int clientId)
        {
            throw new NotImplementedException();
        }

        public int SetPassword(int clientId, string password)
        {
            throw new NotImplementedException();
        }

        public IClient StoreClientInfo(ref int clientId, string lname, string fname, string mname, string username, ClientDemographics demographics, IEnumerable<IPriv> privs, IEnumerable<ICommunity> communities, int technicalFieldId, int orgId, int roleId, int deptId, string email, string phone, bool isManager, bool isFinManager, DateTime? subsidyStart, DateTime? newFacultyStart, int[] addedAddressIds, int[] deletedAddressIds, int[] clientManagerIds, int[] clientAccountIds, out string alert)
        {
            throw new NotImplementedException();
        }

        public ITechnicalField TechnicalField(int clientId)
        {
            throw new NotImplementedException();
        }

        public string TechnicalFieldName(int clientId)
        {
            throw new NotImplementedException();
        }

        public int TotalDaysInLab(int clientId, int roomId, DateTime period)
        {
            throw new NotImplementedException();
        }

        public bool Insert(IClient client)
        {
            var item = Post<ClientItem>("webapi/data/client", client);
            client.ClientID = item.ClientID;
            return true;
        }

        public bool Update(IClient client)
        {
            return Put("webapi/data/client", client);
        }

        public bool UpdatePhysicalAccess(IClient client, out string alert)
        {
            throw new NotImplementedException();
        }

        public ClientDemographics GetClientDemographics(int clientId)
        {
            throw new NotImplementedException();
        }

        public int UpdateClientDemographics(ClientDemographics value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientAccount> GetClientAccounts(int clientId)
        {
            return Get<List<ClientAccountItem>>("webapi/data/client/{clientId}/accounts/active", UrlSegments(new { clientId }));
        }

        public IEnumerable<IClientAccount> GetActiveClientAccounts(int clientId)
        {
            var result = Get<List<ClientAccountItem>>("webapi/data/client/{clientId}/accounts/active", UrlSegments(new { clientId }));
            return result;
        }

        public IEnumerable<IClientAccount> GetActiveClientAccounts(int clientId, DateTime sd, DateTime ed)
        {
            return Get<List<ClientAccountItem>>("webapi/data/client/{clientId}/accounts/active/range", UrlSegments(new { clientId }) & QueryStrings(new { sd = sd.ToString("yyyy-MM-dd"), ed = ed.ToString("yyyy-MM-dd") }));
        }

        public string AccountEmail(int clientId, int accountId)
        {
            throw new NotImplementedException();
        }

        public string AccountPhone(int clientId, int accountId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<GenericListItem> AllActiveManagers()
        {
            return Get<List<GenericListItem>>("webapi/data/client/manager/active/list");
        }

        public IEnumerable<IClient> GetActiveManagers()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> GetActiveManagers(bool includeFinancialManagers)
        {
            throw new NotImplementedException();
        }

        public void Disable(IClient client)
        {
            throw new NotImplementedException();
        }

        public IBillingType GetBillingType(int clientOrgId)
        {
            throw new NotImplementedException();
        }

        public int GetMaxChargeTypeID(int clientId)
        {
            throw new NotImplementedException();
        }

        public IClient GetPrimary(int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> GetClientOrgs(int clientId)
        {
            return Get<List<ClientItem>>("webapi/client/{clientId}/orgs", UrlSegments(new { clientId }));
        }

        public IEnumerable<IClient> GetActiveClientOrgs(int clientId)
        {
            return Get<List<ClientItem>>("webapi/client/{clientId}/orgs/active", UrlSegments(new { clientId }));
        }

        public IEnumerable<IClient> GetActiveClientOrgs(int clientId, DateTime sd, DateTime ed)
        {
            return Get<List<ClientItem>>("webapi/client/{clientId}/org/active/range", UrlSegments(new { clientId }) & QueryStrings(new { sd = sd.ToString("yyyy-MM-dd"), ed = ed.ToString("yyyy-MM-dd") }));
        }

        public IEnumerable<IClient> SelectByClientAccount(int clientId, int accountId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> SelectOrgManagers(int orgId = 0)
        {
            throw new NotImplementedException();
        }

        public void InsertClientRemote(IClientRemote model, DateTime period)
        {
            Post("client/remote", model, QueryStrings(new { period }));
        }

        public void DeleteClientRemote(int clientRemoteId, DateTime period)
        {
            Delete("client/remote/{clientRemoteId}", UrlSegments(new { clientRemoteId }) & QueryStrings(new { period }));
        }

        public IEnumerable<IClientRemote> GetActiveClientRemotes(DateTime sd, DateTime ed)
        {
            return Get<List<ClientRemoteItem>>("client/remote/active/range", QueryStrings(new { sd, ed }));
        }

        public IEnumerable<IPriv> GetPrivs()
        {
            return Get<List<PrivItem>>("webapi/data/client/priv");
        }

        public IEnumerable<ICommunity> GetCommunities()
        {
            return Get<List<CommunityItem>>("webapi/data/client/community");
        }

        public IEnumerable<IClientAccount> GetActiveClientAccounts()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> GetActiveClientOrgs()
        {
            throw new NotImplementedException();
        }

        public IClientPreference GetClientPreference(int clientId, string appName)
        {
            throw new NotImplementedException();
        }

        public IStaffDirectory GetStaffDirectory(string userName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientAccount> GetClientAccounts()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> GetClientOrgs()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> GetActiveClientOrgs(DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientManager> GetClientManagersByManager(int managerOrgId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientManager> GetClientManagersByManaged(int clientOrgId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientAccount> GetClientAccounts(int clientId, int[] accountIds)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientAccount> GetClientAccounts(IClient client)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientAccountAssignment> GetClientAccountAssignments(int managerOrgId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAccount> GetActiveAccounts(int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAccount> GetActiveAccounts(int clientId, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> GetClients(int[] ids)
        {
            throw new NotImplementedException();
        }

        public IMessengerMessage CreateMessage(int clientId, string subject, string body, int parentId, bool disableReply, bool exclusive, bool acknowledgeRequired, bool blockAccess, int accessCutoff)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(int messageId, int[] recipients)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IMessengerRecipient> GetMessages(int clientId, string folder)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ClientListItem> GetClients()
        {
            return Get<List<ClientListItem>>("webapi/data/client/list");
        }

        public bool GetRequirePasswordReset(int clientId)
        {
            throw new NotImplementedException();
        }

        public IPasswordResetRequest GetPasswordResetRequest(string code)
        {
            throw new NotImplementedException();
        }

        public IPasswordResetRequest AddPasswordResetRequest(int clientId)
        {
            throw new NotImplementedException();
        }

        public void CompletePasswordReset(int clientId, string code)
        {
            throw new NotImplementedException();
        }

        public void SetRequirePasswordReset(int clientId, bool value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientAccount> GetActiveClientAccounts(int[] clientIds)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientAccount> GetActiveClientAccounts(string username)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientAccount> GetActiveClientAccounts(string username, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public IStaffDirectory GetStaffDirectory(int staffDirectoryId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IStaffDirectory> GetStaffDirectories(bool? active = true, bool? deleted = false)
        {
            throw new NotImplementedException();
        }

        public void SaveStaffDirectory(IStaffDirectory sd)
        {
            throw new NotImplementedException();
        }
    }
}