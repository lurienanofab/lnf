﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Data;

namespace LNF.Impl.Mappings.Data
{
    public class OAuthClientAuthorizationMap:ClassMap<OAuthClientAuthorization>
    {
        public OAuthClientAuthorizationMap()
        {
            Schema("sselData.dbo");
            Id(x => x.OAuthClientAuthorizationID);
            References(x => x.OAuthClientAudience);
            References(x => x.Client);
            Map(x => x.AuthorizationCode);
            Map(x => x.RedirectUri);
            Map(x => x.State);
            Map(x => x.Expires);
            Map(x => x.IsExchanged);
            Map(x => x.ExchangedOn);
        }
    }
}