using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class ScriptMap : ClassMap<Script>
    {
        internal ScriptMap()
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
