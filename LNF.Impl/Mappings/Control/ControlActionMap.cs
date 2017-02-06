using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Control;

namespace LNF.Impl.Mappings.Control
{
    public class ControlActionMap : ClassMap<ControlAction>
    {
        public ControlActionMap()
        {
            Schema("sselControl.dbo");
            Table("Action");
            Id(x => x.ActionID);
            Map(x => x.ActionName);
            Map(x => x.ActionTableName);
        }
    }
}
