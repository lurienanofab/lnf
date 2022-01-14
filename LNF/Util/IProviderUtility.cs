using LNF.Util.AutoEnd;
using LNF.Util.Serialization;
using LNF.Util.SiteMenu;

namespace LNF.Util
{
    public interface IProviderUtility
    {
        ISerializationUtility Serialization { get; }
        ISiteMenuUtility SiteMenu { get; }
        IAutoEndUtility AutoEnd { get; }
    }
}
