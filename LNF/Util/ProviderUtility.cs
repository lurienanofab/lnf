using LNF.Util.AutoEnd;
using LNF.Util.Encryption;
using LNF.Util.Serialization;
using LNF.Util.SiteMenu;

namespace LNF.Util
{
    public class ProviderUtility : IProviderUtility
    {
        public IEncryptionUtility Encryption { get; }
        public ISerializationUtility Serialization { get; }
        public ISiteMenuUtility SiteMenu { get; }
        public IAutoEndUtility AutoEnd { get; }

        public ProviderUtility(IEncryptionUtility encryption, ISerializationUtility serialization, ISiteMenuUtility siteMenu, IAutoEndUtility autoEnd)
        {
            Encryption = encryption;
            Serialization = serialization;
            SiteMenu = siteMenu;
            AutoEnd = autoEnd;
        }
    }
}
