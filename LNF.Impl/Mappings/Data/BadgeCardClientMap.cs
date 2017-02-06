using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Data
{
    public class BadgeCardClientMap : ClassMap<BadgeCardClient>
    {
        public BadgeCardClientMap()
        {
            Schema("sselData.dbo");
            Table("v_BadgeCardClient");
            ReadOnly();
            Id(x => x.BadgeID, "BADGE_ID");
            Map(x => x.CardNumber, "CARD_NO");
            References(x => x.Client, "BADGE_CLIENTID");
            Map(x => x.UserName, "BADGE_SSEL_UNAME");
            Map(x => x.BadgeExpiration, "BADGE_EXPIRE_DATE");
            Map(x => x.CardExpiration, "CARD_EXPIRE_DATE");
            Map(x => x.CardStatus, "CARD_STATUS");
        }
    }
}
