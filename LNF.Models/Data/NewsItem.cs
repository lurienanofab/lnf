using System;

namespace LNF.Models.Data
{
    public class NewsItem : INews
    {
        public int NewsID { get; set; }
        public IClient NewsCreatedByClient { get; set; }
        public int? NewsUpdatedByClientID { get; set; }
        public byte[] NewsImage { get; set; }
        public string NewsImageFileName { get; set; }
        public string NewsImageContentType { get; set; }
        public string NewsTitle { get; set; }
        public string NewsDescription { get; set; }
        public DateTime NewsCreatedDate { get; set; }
        public DateTime? NewsLastUpdate { get; set; }
        public DateTime? NewsPublishDate { get; set; }
        public DateTime? NewsExpirationDate { get; set; }
        public int? NewsSortOrder { get; set; }
        public bool NewsTicker { get; set; }
        public bool NewsDefault { get; set; }
        public bool NewsActive { get; set; }
        public bool NewsDeleted { get; set; }
    }
}
