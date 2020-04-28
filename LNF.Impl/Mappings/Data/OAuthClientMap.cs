using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class OAuthClientMap : ClassMap<OAuthClient>
    {
        internal OAuthClientMap()
        {
            Schema("sselData.dbo");
            Id(x => x.OAuthClientID);
            Map(x => x.Email);
            Map(x => x.Password);
            Map(x => x.CreatedDateTime); 
            Map(x => x.VerificationType);
            Map(x => x.VerificationCode);
            Map(x => x.VerificationDateTime);
            Map(x => x.Active);
            Map(x => x.Deleted);
            HasMany(x => x.Audiences).KeyColumn("OAuthClientID");
        }
    }
}
