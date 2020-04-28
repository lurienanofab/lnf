using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class ApportionmentClientMap : ClassMap<ApportionmentClient>
    {
        internal ApportionmentClientMap()
        {
            ReadOnly();
            Id(x => x.ClientID);
            Map(x => x.DisplayName);
            Map(x => x.Emails);
            Map(x => x.AccountCount);
        }
    }
}
