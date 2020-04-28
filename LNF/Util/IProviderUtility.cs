using LNF.Util.AutoEnd;
using LNF.Util.Encryption;
using LNF.Util.Serialization;
using LNF.Util.SiteMenu;

namespace LNF.Util
{
    public interface IProviderUtility
    {
        IEncryptionUtility Encryption { get; }
        ISerializationUtility Serialization { get; }
        ISiteMenuUtility SiteMenu { get; }
        IAutoEndUtility AutoEnd { get; }
    }
}
