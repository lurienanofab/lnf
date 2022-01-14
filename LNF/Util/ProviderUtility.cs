using LNF.Util.AutoEnd;
using LNF.Util.Serialization;
using LNF.Util.SiteMenu;

namespace LNF.Util
{
    public class ProviderUtility : IProviderUtility
    {
        public ISerializationUtility Serialization { get; }
        public ISiteMenuUtility SiteMenu { get; }
        public IAutoEndUtility AutoEnd { get; }

        public ProviderUtility(ISerializationUtility serialization, ISiteMenuUtility siteMenu, IAutoEndUtility autoEnd)
        {
            Serialization = serialization;
            SiteMenu = siteMenu;
            AutoEnd = autoEnd;
        }
    }
}
