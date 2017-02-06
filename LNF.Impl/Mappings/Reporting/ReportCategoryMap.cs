using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Reporting;

namespace LNF.Impl.Mappings.Reporting
{
    public class ReportCategoryMap : ClassMap<ReportCategory>
    {
        public ReportCategoryMap()
        {
            Schema("Reporting.dbo");
            Table("Category");
            Id(x => x.CategoryID);
            Map(x => x.Slug);
            Map(x => x.Name);
            Map(x => x.Active);
        }
    }
}
