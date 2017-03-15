using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class ErrorLogMap : ClassMap<ErrorLog>
    {
        internal ErrorLogMap()
        {
            Id(x => x.ErrorLogID);
            Map(x => x.Application);
            Map(x => x.Message);
            Map(x => x.StackTrace);
            Map(x => x.ErrorDateTime);
            Map(x => x.ClientID);
            Map(x => x.PageUrl);
        }
    }
}
