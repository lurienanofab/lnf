using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.GoogleApi
{
    internal class AuthorizationStateMap : ClassMap<GoogleAuthorization>
    {
        internal AuthorizationStateMap()
        {
            Schema("sselData.dbo");
            Id(x => x.GoogleAuthorizationID);
            References(x => x.Client).Not.Nullable();
            Map(x => x.AccessToken).Nullable();
            Map(x => x.AccessTokenExpirationUtc).Nullable();
            Map(x => x.AccessTokenIssueDateUtc).Nullable();
            Map(x => x.Callback).Nullable();
            Map(x => x.RefreshToken).Nullable();
        }
    }
}
