using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Type;
using LNF.Repository.Data;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Data
{
    public class NewsMap : ClassMap<News>
    {
        public NewsMap()
        {
            Schema("sselData.dbo");
            Id(x => x.NewsID);
            References(x => x.NewsCreatedByClient);
            Map(x => x.NewsUpdatedByClientID);
            Map(x => x.NewsImage).CustomType<BinaryBlobType>();
            Map(x => x.NewsImageFileName);
            Map(x => x.NewsImageContentType);
            Map(x => x.NewsTitle);
            Map(x => x.NewsDescription);
            Map(x => x.NewsCreatedDate);
            Map(x => x.NewsLastUpdate);
            Map(x => x.NewsPublishDate);
            Map(x => x.NewsExpirationDate);
            Map(x => x.NewsSortOrder);
            Map(x => x.NewsTicker);
            Map(x => x.NewsDefault);
            Map(x => x.NewsActive);
            Map(x => x.NewsDeleted);
        }
    }
}
