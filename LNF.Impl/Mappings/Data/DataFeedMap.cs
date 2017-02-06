﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Data
{
    public class DataFeedMap : ClassMap<DataFeed>
    {
        public DataFeedMap()
        {
            Schema("sselData.dbo");
            Id(x => x.FeedID);
            Map(x => x.FeedGUID);
            Map(x => x.FeedAlias);
            Map(x => x.FeedName);
            Map(x => x.FeedQuery).Length(int.MaxValue);
            Map(x => x.Private);
            Map(x => x.Active);
            Map(x => x.Deleted);
            Map(x => x.FeedDescription);
            Map(x => x.FeedLink);
            Map(x => x.FeedType);
        }
    }
}
