using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class DryBoxClientMap : ClassMap<DryBoxClient>
    {
        internal DryBoxClientMap()
        {
            Table("v_DryBoxClient");
            ReadOnly();
            Id(x => x.ClientAccountID);
            Map(x => x.ClientOrgID);
            Map(x => x.ClientID);
            Map(x => x.AccountID);
            Map(x => x.OrgID);
            Map(x => x.LName);
            Map(x => x.FName);
            Map(x => x.Email);
            Map(x => x.OrgName);
            Map(x => x.AccountName);
            Map(x => x.ShortCode);
            Map(x => x.OrgManager);
            Map(x => x.OrgFinManager);
            Map(x => x.AccountManager);
            Map(x => x.ClientActive);
            Map(x => x.ClientOrgActive);
            Map(x => x.ClientAccountActive);
            Map(x => x.AccountActive);
            Map(x => x.OrgActive);
            Map(x => x.HasDryBox);
            Map(x => x.ApprovedDate);
        }
    }
}
