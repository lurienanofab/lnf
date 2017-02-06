using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class ErrorLogMap : ClassMap<ErrorLog>
    {
        public ErrorLogMap()
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
