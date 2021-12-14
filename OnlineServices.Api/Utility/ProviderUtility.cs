using LNF.Util;
using LNF.Util.AutoEnd;
using LNF.Util.Encryption;
using LNF.Util.Serialization;
using LNF.Util.SiteMenu;
using RestSharp;

namespace OnlineServices.Api.Utility
{
    public class ProviderUtility : IProviderUtility
    {
        public IEncryptionUtility Encryption { get; }

        public ISerializationUtility Serialization { get; }

        public ISiteMenuUtility SiteMenu { get; }

        public IAutoEndUtility AutoEnd { get; }

        internal ProviderUtility(IRestClient rc)
        {
            Encryption = new EncryptionUtility();
            Serialization = new SerializationUtility();
            SiteMenu = new SiteMenuUtility(rc);
            AutoEnd = new AutoEndUtility(rc);
        }
    }
}
