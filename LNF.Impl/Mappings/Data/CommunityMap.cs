using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Data
{
    public class CommunityMap : ClassMap<Community>
    {
        public CommunityMap()
        {
            Schema("sselData.dbo");
            Id(x => x.CommunityID);
            Map(x => x.CommunityFlag).ReadOnly();
            Map(x => x.CommunityName, "Community");
        }
    }
}
