using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class DryBoxMap : ClassMap<LNF.Data.DryBox>
    {
        internal DryBoxMap()
        {
            Id(x => x.DryBoxID);
            Map(x => x.DryBoxName);
            Map(x => x.Visible);
            Map(x => x.Active);
            Map(x => x.Deleted);
        }
    }
}
