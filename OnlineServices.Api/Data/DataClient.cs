﻿using LNF.Models.Data;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace OnlineServices.Api.Data
{
    public class DataClient : ApiClient
    {
        internal DataClient(ApiClientOptions options) : base(options) { }

        public async Task<IEnumerable<ClientModel>> GetClients(int limit, int skip = 0)
        {
            return await Get<IEnumerable<ClientModel>>(string.Format("data/client?limit={0}&skip={1}", limit, skip));
        }

        public async Task<ClientModel> GetClient(int clientId)
        {
            return await Get<ClientModel>(string.Format("data/client/{0}", clientId));
        }

        public async Task<IEnumerable<ClientModel>> GetActiveClients(ClientPrivilege privs = 0)
        {
            return await Get<IEnumerable<ClientModel>>(string.Format("data/client/active?privs={0}", (int)privs));
        }

        public async Task<IEnumerable<ClientModel>> GetActiveClients(DateTime sd, DateTime ed, ClientPrivilege privs = 0)
        {
            return await Get<IEnumerable<ClientModel>>(string.Format("data/client/active/range?sd={0:yyyy-MM-dd}&ed={1:yyyy-MM-dd}&privs={2}", sd, ed, (int)privs));
        }

        public async Task<ClientModel> AddClient(ClientModel client)
        {
            return await Post<ClientModel>("data/client", client);
        }

        public async Task<bool> UpdateClient(ClientModel client)
        {
            return await Put<bool>("data/client", client);
        }

        public async Task<IEnumerable<ClientAccountModel>> GetClientAccounts(int clientId)
        {
            return await Get<IEnumerable<ClientAccountModel>>(string.Format("data/client/{0}/accounts/active", clientId));
        }

        public async Task<IEnumerable<ClientAccountModel>> GetActiveClientAccounts(int clientId)
        {
            return await Get<IEnumerable<ClientAccountModel>>(string.Format("data/client/{0}/accounts/active", clientId));
        }

        public async Task<IEnumerable<ClientAccountModel>> GetActiveClientAccounts(int clientId, DateTime sd, DateTime ed)
        {
            return await Get<IEnumerable<ClientAccountModel>>(string.Format("data/client/{0}/accounts/active/range?sd={1:yyyy-MM-dd}&ed={2:yyyy-MM-dd}", clientId, sd, ed));
        }

        public async Task<IEnumerable<ClientRemoteModel>> GetActiveClientRemotes(DateTime sd, DateTime ed)
        {
            return await Get<IEnumerable<ClientRemoteModel>>(string.Format("data/client/remote/active?sd={0:yyyy-MM-dd}&ed={1:yyyy-MM-dd}", sd, ed));
        }

        public async Task<ClientRemoteModel> AddClientRemote(ClientRemoteModel model, DateTime period)
        {
            return await Post<ClientRemoteModel>(string.Format("data/client/remote?period={0:yyyy-MM-dd}", period), model);
        }

        public async Task<bool> DeleteClientRemote(int clientRemoteId)
        {
            return await Delete(string.Format("data/client/remote/{0}", clientRemoteId));
        }

        public async Task<IEnumerable<DryBoxModel>> GetDryBoxes()
        {
            return await Get<IEnumerable<DryBoxModel>>("data/drybox");
        }

        public async Task<bool> UpdateDryBox(DryBoxModel model)
        {
            return await Put<DryBoxModel>("data/drybox", model);
        }

        public async Task<IEnumerable<ServiceLogModel>> GetServiceLogs(Guid? id = null, string service = null, string subject = null)
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

            return await Get<IEnumerable<ServiceLogModel>>(url);
        }

        public async Task<ServiceLogModel> AddServiceLog(ServiceLogModel model)
        {
            return await Post<ServiceLogModel>("data/servicelog", model);
        }

        public async Task<bool> UpdateServiceLog(Guid id, string data)
        {
            IDictionary<string, string> postData = new Dictionary<string, string>();
            postData.Add("", data);
            HttpContent content = new FormUrlEncodedContent(postData);
            return await Put<bool>(string.Format("data/servicelog/{0}", id), content);
        }
    }
}