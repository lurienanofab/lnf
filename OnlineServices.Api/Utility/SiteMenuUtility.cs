using LNF.Util.SiteMenu;
using RestSharp;
using System;

namespace OnlineServices.Api.Utility
{
    public class SiteMenuUtility : ApiClient, ISiteMenuUtility
    {
        internal SiteMenuUtility(IRestClient rc) : base(rc) { }

        public string GetSiteMenu(int clientId, string target = null)
        {
            if (clientId <= 0)
                throw new ArgumentOutOfRangeException("clientId");

            var result = Get("webapi/data/ajax/menu", QueryStrings(new { clientId, target }));

            return result;
        }

        public string GetSiteMenu(string username, string target = null)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("usernmae");

            var result = Get("webapi/data/ajax/menu", QueryStrings(new { username, target }));

            return result;
        }
    }
}
