using LNF.DataAccess;
using System;

namespace LNF.Data
{
    public class News : IDataItem
    {
        public virtual int NewsID { get; set; }
        public virtual int NewsCreatedByClientID { get; set; }
        public virtual int? NewsUpdatedByClientID { get; set; }
        public virtual byte[] NewsImage { get; set; }
        public virtual string NewsImageFileName { get; set; }
        public virtual string NewsImageContentType { get; set; }
        public virtual string NewsTitle { get; set; }
        public virtual string NewsDescription { get; set; }
        public virtual DateTime NewsCreatedDate { get; set; }
        public virtual DateTime? NewsLastUpdate { get; set; }
        public virtual DateTime? NewsPublishDate { get; set; }
        public virtual DateTime? NewsExpirationDate { get; set; }
        public virtual int? NewsSortOrder { get; set; }
        public virtual bool NewsTicker { get; set; }
        public virtual bool NewsDefault { get; set; }
        public virtual bool NewsActive { get; set; }
        public virtual bool NewsDeleted { get; set; }
    }
}
