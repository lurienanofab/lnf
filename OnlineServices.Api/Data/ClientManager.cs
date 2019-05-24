using LNF.Models.Billing;
using LNF.Models.Data;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Data
{
    public class ClientManager : ApiClient, IClientManager
    {
        public IEnumerable<IAccount> ActiveAccounts(int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAccount> ActiveAccounts(int clientId, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientAccount> ActiveClientAccounts(int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientAccount> ActiveClientAccounts(int clientId, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> ActiveClientOrgs(int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> ActiveClientOrgs(int clientId, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

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

        public IEnumerable<IClientAccount> ClientAccounts(int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> ClientOrgs(int clientId)
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
            return Get<ClientItem>("webapi/data/client", QueryStrings(new { username }));
        }

        public IClient GetClient(int clientId)
        {
            return Get<ClientItem>("webapi/data/client", QueryStrings(new { clientId }));
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

        public IClient StoreClientInfo(ref int clientId, string lname, string fname, string mname, string username, IClientDemographics demographics, IEnumerable<IPriv> privs, IEnumerable<ICommunity> communities, int technicalFieldId, int orgId, int roleId, int deptId, string email, string phone, bool isManager, bool isFinManager, DateTime? subsidyStart, DateTime? newFacultyStart, int[] addedAddressIds, int[] deletedAddressIds, int[] clientManagerIds, int[] clientAccountIds, out string alert)
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

        public IClientDemographics GetClientDemographics(int clientId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateClientDemographics(IClientDemographics value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientAccount> GetClientAccounts(int clientId)
        {
            return Get<List<ClientAccountItem>>("webapi/data/client/{clientId}/accounts/active", UrlSegments(new { clientId }));
        }

        public IEnumerable<IClientAccount> GetActiveClientAccounts(int clientId)
        {
            return Get<List<ClientAccountItem>>("webapi/data/client/{clientId}/accounts/active", UrlSegments(new { clientId }));
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

        public IEnumerable<IClient> AllActiveManagers()
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
    }
}