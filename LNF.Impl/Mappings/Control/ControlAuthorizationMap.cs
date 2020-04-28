using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Control;

namespace LNF.Impl.Mappings.Control
{
    internal class ControlAuthorizationMap : ClassMap<ControlAuthorization>
    {
        internal ControlAuthorizationMap()
        {
            Schema("sselControl.dbo");
            Table("[Authorization]");
            CompositeId()
                .KeyProperty(x => x.ActionID)
                .KeyProperty(x => x.ActionInstanceID)
                .KeyProperty(x => x.ClientID)
                .KeyProperty(x => x.Location);
        }
    }
}
