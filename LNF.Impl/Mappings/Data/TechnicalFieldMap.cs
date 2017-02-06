using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Data
{
    public class TechnicalFieldMap : ClassMap<TechnicalField>
    {
        public TechnicalFieldMap()
        {
            Schema("sselData.dbo");
            Id(x => x.TechnicalFieldID);
            Map(x => x.TechnicalFieldName, "TechnicalField");
        }
    }
}
