using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class PasswordResetRequestMap : ClassMap<PasswordResetRequest>
    {
        internal PasswordResetRequestMap()
        {
            Schema("sselData.dbo");
            Id(x => x.PasswordResetRequestID);
            Map(x => x.ClientID);
            Map(x => x.RequestDateTime);
            Map(x => x.ResetCode);
            Map(x => x.ResetDateTime);
        }
    }
}
