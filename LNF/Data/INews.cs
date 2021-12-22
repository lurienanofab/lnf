using System;

namespace LNF.Data
{
    public interface INews
    {
        bool NewsActive { get; set; }
        int NewsCreatedByClientID { get; set; }
        DateTime NewsCreatedDate { get; set; }
        bool NewsDefault { get; set; }
        bool NewsDeleted { get; set; }
        string NewsDescription { get; set; }
        DateTime? NewsExpirationDate { get; set; }
        int NewsID { get; set; }
        byte[] NewsImage { get; set; }
        string NewsImageContentType { get; set; }
        string NewsImageFileName { get; set; }
        DateTime? NewsLastUpdate { get; set; }
        DateTime? NewsPublishDate { get; set; }
        int? NewsSortOrder { get; set; }
        bool NewsTicker { get; set; }
        string NewsTitle { get; set; }
        int? NewsUpdatedByClientID { get; set; }
    }
}