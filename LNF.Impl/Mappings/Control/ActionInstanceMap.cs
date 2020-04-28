using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Control;

namespace LNF.Impl.Mappings.Control
{
    internal class ActionInstanceMap:ClassMap<ActionInstance>
    {
        internal ActionInstanceMap()
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
