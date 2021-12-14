using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class ClientMap : ClassMap<Client>
    {
        internal ClientMap()
        {
            Schema("sselData.dbo");
            Id(x => x.ClientID);
            Map(x => x.FName);
            Map(x => x.MName);
            Map(x => x.LName);
            Map(x => x.UserName).Not.Nullable();
            Map(x => x.Privs);
            Map(x => x.Communities);
            Map(x => x.TechnicalInterestID);
            Map(x => x.Active);
            Map(x => x.IsChecked, "isChecked");
            Map(x => x.IsSafetyTest, "isSafetyTest");
            Map(x => x.RequirePasswordReset);
        }
    }
}
