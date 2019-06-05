using LNF.Models.Data;
using System;
using System.Collections.Generic;

namespace LNF.Repository.Data
{
    public class DataFeed : IDataItem
    {
        public virtual int FeedID { get; set; }
        public virtual Guid FeedGUID { get; set; }
        public virtual string FeedAlias { get; set; }
        public virtual string FeedName { get; set; }
        public virtual string FeedQuery { get; set; }
        public virtual string DefaultParameters { get; set; }
        public virtual bool Private { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual string FeedDescription { get; set; }
        public virtual string FeedLink { get; set; }
        public virtual DataFeedType FeedType { get; set; }

        /// <summary>
        /// Gets a dictionary that contains default parameters (if any) merged with the given parameters, overwritting defaults.
        /// </summary>
        public virtual void ApplyDefaultParameters(IDictionary<object, object> parameters) => DataFeedItem.ApplyDefaultParameters(DefaultParameters, parameters);
    }
}
