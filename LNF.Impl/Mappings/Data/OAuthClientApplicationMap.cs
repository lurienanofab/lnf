using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Data;
using LNF.Repository;

namespace LNF.Impl.Mappings.Data
{
    public class OAuthClientApplicationMap : ClassMap<OAuthClientAudience>
    {
        public OAuthClientApplicationMap()
        {
            Schema("sselData.dbo");
            Id(x => x.OAuthClientAudienceID);
            References(x => x.OAuthClient);
            Map(x => x.ApplicationName);
            Map(x => x.ApplicationDescription);
            Map(x => x.Configuration);
            Map(x => x.AudienceId);
            Map(x => x.AudienceSecret);
            Map(x => x.CreatedDateTime);
            Map(x => x.Active);
            Map(x => x.Deleted);
        }
    }
}
