using FluentNHibernate.Mapping;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    public class ApportionmentClientMap : ClassMap<ApportionmentClient>
    {
        public ApportionmentClientMap()
        {
            ReadOnly();
            Id(x => x.ClientID);
            Map(x => x.DisplayName);
            Map(x => x.Emails);
            Map(x => x.AccountCount);
        }
    }
}
