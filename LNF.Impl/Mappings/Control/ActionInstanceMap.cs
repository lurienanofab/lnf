using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Control;
using LNF.Repository.Control;

namespace LNF.Impl.Mappings.Control
{
    public class ActionInstanceMap:ClassMap<ActionInstance>
    {
        public ActionInstanceMap()
        {
            Schema("sselControl.dbo");
            Table("v_ActionInstance");
            ReadOnly();
            Id(x => x.Index, "[Index]");
            Map(x => x.Point);
            Map(x => x.ActionID);
            Map(x => x.Name);
            Map(x => x.ActionName);
        }
    }
}
