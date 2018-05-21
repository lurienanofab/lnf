using LNF.Models;
using LNF.Models.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using LNF.Models.Data.Utility.BillingChecks;

namespace OnlineServices.Api.Data
{
    public class DataClient : ApiClient
    {
        public DataClient() : base(ConfigurationManager.AppSettings["ApiHost"]) { }

        public async Task<IEnumerable<ClientItem>> GetClients(int limit, int skip = 0)
        {
            return await Get<IEnumerable<ClientItem>>(string.Format("data/client?limit={0}&skip={1}", limit, skip));
        }

        public async Task<ClientItem> GetClient(int clientId)
        {
            return await Get<ClientItem>(string.Format("data/client/{0}", clientId));
        }

        public async Task<IEnumerable<ClientItem>> GetActiveClients(ClientPrivilege privs = 0)
        {
            return await Get<IEnumerable<ClientItem>>(string.Format("data/client/active?privs={0}", (int)privs));
        }

        public async Task<IEnumerable<ClientItem>> GetActiveClients(DateTime sd, DateTime ed, ClientPrivilege privs = 0)
        {
            return await Get<IEnumerable<ClientItem>>(string.Format("data/client/active/range?sd={0:yyyy-MM-dd}&ed={1:yyyy-MM-dd}&privs={2}", sd, ed, (int)privs));
        }

        public async Task<ClientItem> AddClient(ClientItem client)
        {
            return await Post<ClientItem>("data/client", client);
        }

        public async Task<bool> UpdateClient(ClientItem client)
        {
            return await Put<bool>("data/client", client);
        }

        public async Task<IEnumerable<ClientAccountItem>> GetClientAccounts(int clientId)
        {
            return await Get<IEnumerable<ClientAccountItem>>(string.Format("data/client/{0}/accounts/active", clientId));
        }

        public async Task<IEnumerable<ClientAccountItem>> GetActiveClientAccounts(int clientId)
        {
            return await Get<IEnumerable<ClientAccountItem>>(string.Format("data/client/{0}/accounts/active", clientId));
        }

        public async Task<IEnumerable<ClientAccountItem>> GetActiveClientAccounts(int clientId, DateTime sd, DateTime ed)
        {
            return await Get<IEnumerable<ClientAccountItem>>(string.Format("data/client/{0}/accounts/active/range?sd={1:yyyy-MM-dd}&ed={2:yyyy-MM-dd}", clientId, sd, ed));
        }

        public async Task<IEnumerable<ClientRemoteItem>> GetActiveClientRemotes(DateTime sd, DateTime ed)
        {
            return await Get<IEnumerable<ClientRemoteItem>>(string.Format("data/client/remote/active?sd={0:yyyy-MM-dd}&ed={1:yyyy-MM-dd}", sd, ed));
        }

        public async Task<ClientRemoteItem> AddClientRemote(ClientRemoteItem model, DateTime period)
        {
            return await Post<ClientRemoteItem>(string.Format("data/client/remote?period={0:yyyy-MM-dd}", period), model);
        }

        public async Task<bool> DeleteClientRemote(int clientRemoteId)
        {
            return await Delete(string.Format("data/client/remote/{0}", clientRemoteId));
        }

        public async Task<IEnumerable<DryBoxItem>> GetDryBoxes()
        {
            return await Get<IEnumerable<DryBoxItem>>("data/drybox");
        }

        public async Task<bool> UpdateDryBox(DryBoxItem model)
        {
            return await Put<DryBoxItem>("data/drybox", model);
        }

        public async Task<IEnumerable<ServiceLogItem>> GetServiceLogs(Guid? id = null, string service = null, string subject = null)
        {
            string url = "data/servicelog";

            if (id.HasValue)
                url += "/" + id.ToString().ToLower();

            string amp = "?";

            if (!string.IsNullOrEmpty(service))
            {
                url += amp + "service=" + service;
                amp = "&";
            }

            if (!string.IsNullOrEmpty(subject))
            {
                url += amp + "subject=" + subject;
                amp = "&";
            }

            return await Get<IEnumerable<ServiceLogItem>>(url);
        }

        public async Task<ServiceLogItem> AddServiceLog(ServiceLogItem model)
        {
            return await Post<ServiceLogItem>("data/servicelog", model);
        }

        public async Task<bool> UpdateServiceLog(Guid id, string data)
        {
            IDictionary<string, string> postData = new Dictionary<string, string> { { "", data } };
            HttpContent content = new FormUrlEncodedContent(postData);
            return await Put<bool>(string.Format("data/servicelog/{0}", id), content);
        }

        public async Task<IEnumerable<AutoEndProblem>> GetAutoEndProblems(DateTime period)
        {
            var result = await Get<IEnumerable<AutoEndProblem>>(string.Format("data/utility/billing-checks/auto-end-problems?period={0:yyyy-MM-dd}", period));
            return result;
        }

        public async Task<int> FixAllAutoEndProblems(DateTime period)
        {
            var result = await Get<int>(string.Format("data/utility/billing-checks/auto-end-problems/fix-all?period={0:yyyy-MM-dd}", period));
            return result;
        }

        public async Task<int> FixAutoEndProblem(DateTime period, int reservationId)
        {
            if (reservationId <= 0)
                throw new ArgumentOutOfRangeException("reservationId");

            var result = await Get<int>(string.Format("data/utility/billing-checks/auto-end-problems/fix?period={0:yyyy-MM-dd}&reservationId={1}", period, reservationId));

            return result;
        }

        public async Task<string> GetSiteMenu(int clientId)
        {
            if (clientId <= 0)
                throw new ArgumentOutOfRangeException("clientId");

            var result = await Get<string>(string.Format("data/ajax/menu?clientId={0}", clientId));

            return result;
        }
    }
}
