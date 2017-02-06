using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Data
{
    public class ScriptMap : ClassMap<Script>
    {
        public ScriptMap()
        {
            Schema("sselData.dbo");
            Id(x => x.ScriptID);
            Map(x => x.ScriptName);
            Map(x => x.ScriptText);
            Map(x => x.CreatedOn);
            Map(x => x.ModifiedOn);
            References(x => x.CreatedBy, "CreatedByClientID");
            References(x => x.ModifiedBy, "ModifiedByClientID");
            Map(x => x.Active);
            Map(x => x.Deleted);
        }
    }
}
