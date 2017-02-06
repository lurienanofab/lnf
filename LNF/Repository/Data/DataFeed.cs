using System;
using System.Collections.Generic;
using System.Text;

namespace LNF.Repository.Data
{
    public enum DataFeedType
    {
        SQL = 0,
        Python = 1
    }

    public class DataFeed: IDataItem
    {
        public virtual int FeedID { get; set; }
        public virtual Guid FeedGUID { get; set; }
        public virtual string FeedAlias { get; set; }
        public virtual string FeedName { get; set; }
        public virtual string FeedQuery { get; set; }
        public virtual bool Private { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual string FeedDescription { get; set; }
        public virtual string FeedLink { get; set; }
        public virtual DataFeedType FeedType { get; set; }
    }
}
