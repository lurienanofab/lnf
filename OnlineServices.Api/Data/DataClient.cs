using LNF.Models;
using LNF.Models.Data;
using LNF.Models.Data.Utility.BillingChecks;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineServices.Api.Data
{
    public class DataClient : ApiClient, IDataService
    {
        public DataClient() : base(GetApiBaseUrl()) { }

        public IEnumerable<ClientItem> GetClients(int limit, int skip = 0)
        {
            return Get<List<ClientItem>>("webapi/data/client", QueryStrings(new { limit, skip }));
        }

        public IEnumerable<ClientItem> GetActiveClients(ClientPrivilege privs = 0)
        {
            return Get<List<ClientItem>>("webapi/data/client/active", QueryStrings(new { privs = (int)privs }));
        }

        public IEnumerable<ClientItem> GetActiveClientsInRange(DateTime sd, DateTime ed, ClientPrivilege privs = 0)
        {
            return Get<List<ClientItem>>("webapi/data/client/active/range", QueryStrings(new { sd = sd.ToString("yyyy-MM-dd"), ed = ed.ToString("yyyy-MM-dd"), privs = (int)privs }));
        }

        public ClientItem GetClient(int clientId)
        {
            return Get<ClientItem>("webapi/data/client", QueryStrings(new { clientId }));
        }

        public ClientItem GetClient(string username)
        {
            return Get<ClientItem>("webapi/data/client", QueryStrings(new { username }));
        }

        public ClientDemographics GetClientDemographics(int clientId)
        {
            return Get<ClientDemographics>("webapi/data/client/{clientId}/demographics", UrlSegments(new { clientId }));
        }

        public ClientItem InsertClient(ClientItem client)
        {
            return Post<ClientItem>("webapi/data/client", client);
        }

        public bool UpdateClient(ClientItem client)
        {
            return Put("webapi/data/client", client);
        }

        public IEnumerable<ClientAccountItem> GetClientAccounts(int clientId)
        {
            return Get<List<ClientAccountItem>>("webapi/data/client/{clientId}/accounts/active", UrlSegments(new { clientId }));
        }

        public IEnumerable<ClientAccountItem> GetActiveClientAccounts(int clientId)
        {
            return Get<List<ClientAccountItem>>("webapi/data/client/{clientId}/accounts/active", UrlSegments(new { clientId }));
        }

        public IEnumerable<ClientAccountItem> GetActiveClientAccountsInRange(int clientId, DateTime sd, DateTime ed)
        {
            return Get<List<ClientAccountItem>>("webapi/data/client/{clientId}/accounts/active/range", UrlSegments(new { clientId }) & QueryStrings(new { sd = sd.ToString("yyyy-MM-dd"), ed = ed.ToString("yyyy-MM-dd") }));
        }

        public IEnumerable<ClientItem> GetClientOrgs(int clientId)
        {
            return Get<List<ClientItem>>("webapi/client/{clientId}/orgs", UrlSegments(new { clientId }));
        }

        public IEnumerable<ClientItem> GetActiveClientOrgs(int clientId)
        {
            return Get<List<ClientItem>>("webapi/client/{clientId}/orgs/active", UrlSegments(new { clientId }));
        }

        public IEnumerable<ClientItem> GetActiveClientOrgsInRange(int clientId, DateTime sd, DateTime ed)
        {

            return Get<List<ClientItem>>("webapi/client/{clientId}/org/active/range", UrlSegments(new { clientId }) & QueryStrings(new { sd = sd.ToString("yyyy-MM-dd"), ed = ed.ToString("yyyy-MM-dd") }));
        }

        public IEnumerable<ClientRemoteItem> GetActiveClientRemotesInRange(DateTime sd, DateTime ed)
        {
            return Get<List<ClientRemoteItem>>("webapi/data/client/remote/active/range", QueryStrings(new { sd = sd.ToString("yyyy-MM-dd"), ed = ed.ToString("yyyy-MM-dd") }));
        }

        public ClientRemoteItem InsertClientRemote(ClientRemoteItem model, DateTime period)
        {
            return Post<ClientRemoteItem>("webapi/data/client/remote", model, QueryStrings(new { period = period.ToString("yyyy-MM-dd") }));
        }

        public int DeleteClientRemote(int clientRemoteId)
        {
            return Delete("webapi/data/client/remote/{clientRemoteId}", UrlSegments(new { clientRemoteId }));
        }

        public IEnumerable<CostItem> GetCosts(int limit, int skip = 0)
        {
            return Get<List<CostItem>>("webapi/data/cost", QueryStrings(new { limit, skip }));
        }

        public CostItem GetCost(int costId)
        {
            return Get<CostItem>("webapi/data/cost/{costId}", UrlSegments(new { costId }));
        }

        public IEnumerable<CostItem> GetResourceCosts(int resourceId, DateTime? cutoff = null, int? chargeTypeId = null)
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

        public IEnumerable<DryBoxItem> GetDryBoxes()
        {
            return Get<List<DryBoxItem>>("webapi/data/drybox");
        }

        public bool UpdateDryBox(DryBoxItem model)
        {
            return Put("webapi/data/drybox", model);
        }

        public IEnumerable<ServiceLogItem> GetServiceLogs(Guid? id = null, string service = null, string subject = null)
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

        public ServiceLogItem InsertServiceLog(ServiceLogItem model)
        {
            return Post<ServiceLogItem>("webapi/data/servicelog", model);
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

        public AccountItem GetAccount(int accountId)
        {
            return Get<AccountItem>("webapi/data/account/{accountId}", UrlSegments(new { accountId }));
        }
    }
}
