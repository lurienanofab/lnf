using LNF.Util.SiteMenu;
using RestSharp;
using System;
using System.Configuration;

namespace LNF.Impl.Data
{
    public class SiteMenuUtility : ISiteMenuUtility
    {
        public string GetSiteMenu(int clientId, string target = null) => GetContent("clientId", clientId, target);

        public string GetSiteMenu(string username, string target = null) => GetContent("username", username, target);

        private string GetContent(string paramName, object paramValue, string target)
        {
            return GetRestClient()
                .Execute(GetRestRequest(target)
                .AddQueryParameter(paramName, Convert.ToString(paramValue)))
                .Content;
        }

        private IRestClient GetRestClient()
        {
            var baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];

            if (string.IsNullOrEmpty(baseUrl))
                throw new Exception("Missing required appSetting: ApiBaseUrl");

            return new RestClient(baseUrl);
        }

        private IRestRequest GetRestRequest(string target)
        {
            var req = new RestRequest("webapi/data/ajax/menu", Method.GET, DataFormat.Json);

            if (!string.IsNullOrEmpty(target))
                req.AddQueryParameter("target", target);

            return req;
        }
    }
}
