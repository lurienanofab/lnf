using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Reporting;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Reporting
{
    public class ReportMap : ClassMap<Report>
    {
        public ReportMap()
        {
            Schema("Reporting.dbo");
            Id(x => x.ReportID);
            References(x => x.Category).Not.Nullable();
            Map(x => x.Slug).Not.Nullable();
            Map(x => x.Name).Not.Nullable();
            Map(x => x.FeedAlias).Not.Nullable();
            Map(x => x.Active).Not.Nullable();
        }
    }
}
