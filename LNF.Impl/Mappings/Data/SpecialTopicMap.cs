using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Data
{
    public class SpecialTopicMap : ClassMap<SpecialTopic>
    {
        public SpecialTopicMap()
        {
            Schema("sselData.dbo");
            Id(x => x.SpecialTopicID);
            Map(x => x.SpecialTopicName, "SpecialTopic");
        }
    }
}
