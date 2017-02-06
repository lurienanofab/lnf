using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Control;

namespace LNF.Impl.Mappings.Control
{
    public class ControlAuthorizationMap : ClassMap<ControlAuthorization>
    {
        public ControlAuthorizationMap()
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
