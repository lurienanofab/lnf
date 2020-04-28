using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class OAuthClientAuthorizationMap:ClassMap<OAuthClientAuthorization>
    {
        internal OAuthClientAuthorizationMap()
        {
            Schema("sselData.dbo");
            Id(x => x.OAuthClientAuthorizationID);
            References(x => x.OAuthClientAudience);
            References(x => x.Client);
            Map(x => x.AuthorizationCode);
            Map(x => x.RedirectUri);
            Map(x => x.State);
            Map(x => x.Expires);
            Map(x => x.IsExchanged);
            Map(x => x.ExchangedOn);
        }
    }
}
