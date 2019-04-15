using LNF.Models;
using LNF.Models.Data;
using LNF.Models.Data.Utility.BillingChecks;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineServices.Api.Data
{
    public class DataService : ApiClient, IDataService
    {
        public IClientManager Client { get; }
        public IAccountManager Account { get; }
        public IChargeTypeManager ChargeType { get; set; }
        public IRoomManager Room { get; set; }
        public IClientRemoteManager ClientRemote { get; set; }

        public DataService(IClientManager clientManager, IAccountManager accountManager, IChargeTypeManager chargeTypeManager, IRoomManager roomManager, IClientRemoteManager clientRemote) : base(GetApiBaseUrl())
        {
            Client = clientManager;
            Account = accountManager;
            ChargeType = chargeTypeManager;
            Room = roomManager;
            ClientRemote = clientRemote;
        }

        public IEnumerable<IClient> GetClients(int limit, int skip = 0)
        {
            return Get<List<ClientItem>>("webapi/data/client", QueryStrings(new { limit, skip }));
        }

        public IEnumerable<IClient> GetActiveClients(ClientPrivilege privs = 0)
        {
            return Get<List<ClientItem>>("webapi/data/client/active", QueryStrings(new { privs = (int)privs }));
        }

        public IEnumerable<IClient> GetActiveClientsInRange(DateTime sd, DateTime ed, ClientPrivilege privs = 0)
        {
            return Get<List<ClientItem>>("webapi/data/client/active/range", QueryStrings(new { sd = sd.ToString("yyyy-MM-dd"), ed = ed.ToString("yyyy-MM-dd"), privs = (int)privs }));
        }

        public IClient GetClient(int clientId)
        {
            return Get<ClientItem>("webapi/data/client", QueryStrings(new { clientId }));
        }

        public IClient GetClient(string username)
        {
            return Get<ClientItem>("webapi/data/client", QueryStrings(new { username }));
        }

        public IClientDemographics GetClientDemographics(int clientId)
        {
            return Get<ClientDemographics>("webapi/data/client/{clientId}/demographics", UrlSegments(new { clientId }));
        }

        public bool InsertClient(IClient client)
        {
            var item = Post<ClientItem>("webapi/data/client", client);
            client.ClientID = item.ClientID;
            return true;
        }

        public bool UpdateClient(IClient client)
        {
            return Put("webapi/data/client", client);
        }

        public IEnumerable<IClientAccount> GetClientAccounts(int clientId)
        {
            return Get<List<IClientAccount>>("webapi/data/client/{clientId}/accounts/active", UrlSegments(new { clientId }));
        }

        public IEnumerable<IClientAccount> GetActiveClientAccounts(int clientId)
        {
            return Get<List<ClientAccountItem>>("webapi/data/client/{clientId}/accounts/active", UrlSegments(new { clientId }));
        }

        public IEnumerable<IClientAccount> GetActiveClientAccountsInRange(int clientId, DateTime sd, DateTime ed)
        {
            return Get<List<ClientAccountItem>>("webapi/data/client/{clientId}/accounts/active/range", UrlSegments(new { clientId }) & QueryStrings(new { sd = sd.ToString("yyyy-MM-dd"), ed = ed.ToString("yyyy-MM-dd") }));
        }

        public IEnumerable<IClient> GetClientOrgs(int clientId)
        {
            return Get<List<ClientItem>>("webapi/client/{clientId}/orgs", UrlSegments(new { clientId }));
        }

        public IEnumerable<IClient> GetActiveClientOrgs(int clientId)
        {
            return Get<List<ClientItem>>("webapi/client/{clientId}/orgs/active", UrlSegments(new { clientId }));
        }

        public IEnumerable<IClient> GetActiveClientOrgsInRange(int clientId, DateTime sd, DateTime ed)
        {

            return Get<List<ClientItem>>("webapi/client/{clientId}/org/active/range", UrlSegments(new { clientId }) & QueryStrings(new { sd = sd.ToString("yyyy-MM-dd"), ed = ed.ToString("yyyy-MM-dd") }));
        }

        public IEnumerable<IClientRemote> GetActiveClientRemotesInRange(DateTime sd, DateTime ed)
        {
            return Get<List<ClientRemoteItem>>("webapi/data/client/remote/active/range", QueryStrings(new { sd = sd.ToString("yyyy-MM-dd"), ed = ed.ToString("yyyy-MM-dd") }));
        }

        public bool InsertClientRemote(IClientRemote model, DateTime period)
        {
            var item = Post<ClientRemoteItem>("webapi/data/client/remote", model, QueryStrings(new { period = period.ToString("yyyy-MM-dd") }));
            model.ClientRemoteID = item.ClientRemoteID;
            return true;
        }

        public int DeleteClientRemote(int clientRemoteId)
        {
            return Delete("webapi/data/client/remote/{clientRemoteId}", UrlSegments(new { clientRemoteId }));
        }

        public IEnumerable<ICost> GetCosts(int limit, int skip = 0)
        {
            return Get<List<CostItem>>("webapi/data/cost", QueryStrings(new { limit, skip }));
        }

        public ICost GetCost(int costId)
        {
            return Get<CostItem>("webapi/data/cost/{costId}", UrlSegments(new { costId }));
        }

        public IEnumerable<ICost> GetResourceCosts(int resourceId, DateTime? cutoff = null, int? chargeTypeId = null)
        {
            var pc = new ParameterCollection
            {
                { "resourceId", resourceId, ParameterType.UrlSegment }
            };

            if (cutoff.HasValue)
                pc.Add("cutoff", cutoff.Value.ToString("yyyy-MM-dd"), ParameterType.QueryString);

            if (chargeTypeId.HasValue)
                pc.Add("chargeTypeId", chargeTypeId.Value, ParameterType.QueryString);

            return Get<List<CostItem>>("webapi/data/cost/resource/{resourceId}", pc);
        }

        public IEnumerable<IDryBox> GetDryBoxes()
        {
            return Get<List<DryBoxItem>>("webapi/data/drybox");
        }

        public bool UpdateDryBox(IDryBox model)
        {
            return Put("webapi/data/drybox", model);
        }

        public IEnumerable<IServiceLog> GetServiceLogs(Guid? id = null, string service = null, string subject = null)
        {
            string url = "webapi/data/servicelog";

            ParameterCollection parameters = new ParameterCollection();

            if (id.HasValue)
            {
                url += "/{id}";
                parameters.Add("id", id.ToString().ToLower(), ParameterType.UrlSegment);
            }

            if (!string.IsNullOrEmpty(service))
            {
                parameters.Add("service", service, ParameterType.QueryString);
            }

            if (!string.IsNullOrEmpty(subject))
            {
                parameters.Add("subject", subject, ParameterType.QueryString);
            }

            return Get<List<ServiceLogItem>>(url, parameters);
        }

        public bool InsertServiceLog(IServiceLog model)
        {
            var item = Post<ServiceLogItem>("webapi/data/servicelog", model);
            model.ServiceLogID = item.ServiceLogID;
            return true;
        }

        public bool UpdateServiceLog(Guid id, string data)
        {
            IDictionary<string, string> postData = new Dictionary<string, string> { { "", data } };
            ParameterCollection parameters = new ParameterCollection { postData.Select(x => new Parameter(x.Key, x.Value, ParameterType.RequestBody)) };
            return Put(string.Format("webapi/data/servicelog/{0}", id), parameters);
        }

        public IEnumerable<AutoEndProblem> GetAutoEndProblems(DateTime period)
        {
            var result = Get<List<AutoEndProblem>>("webapi/data/utility/billing-checks/auto-end-problems", QueryStrings(new { period = period.ToString("yyyy-MM-dd") }));
            return result;
        }

        public int FixAllAutoEndProblems(DateTime period)
        {
            var result = Get<int>("webapi/data/utility/billing-checks/auto-end-problems/fix-all", QueryStrings(new { period = period.ToString("yyyy-MM-dd") }));
            return result;
        }

        public int FixAutoEndProblem(DateTime period, int reservationId)
        {
            if (reservationId <= 0)
                throw new ArgumentOutOfRangeException("reservationId");

            var result = Get<int>("webapi/data/utility/billing-checks/auto-end-problems/fix", QueryStrings(new { period = period.ToString("yyyy-MM-dd"), reservationId }));

            return result;
        }

        public string GetSiteMenu(int clientId)
        {
            if (clientId <= 0)
                throw new ArgumentOutOfRangeException("clientId");

            var result = Get("webapi/data/ajax/menu", QueryStrings(new { clientId }));

            return result;
        }

        public IAccount GetAccount(int accountId)
        {
            return Get<AccountItem>("webapi/data/account/{accountId}", UrlSegments(new { accountId }));
        }
    }
}
