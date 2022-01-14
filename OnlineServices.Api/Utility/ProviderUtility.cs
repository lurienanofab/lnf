using LNF.Util;
using LNF.Util.AutoEnd;
using LNF.Util.Serialization;
using LNF.Util.SiteMenu;
using RestSharp;

namespace OnlineServices.Api.Utility
{
    public class ProviderUtility : IProviderUtility
    {

        public ISerializationUtility Serialization { get; }

        public ISiteMenuUtility SiteMenu { get; }

        public IAutoEndUtility AutoEnd { get; }

        internal ProviderUtility(IRestClient rc)
        {
            Serialization = new SerializationUtility();
            SiteMenu = new SiteMenuUtility(rc);
            AutoEnd = new AutoEndUtility(rc);
        }
    }
}
