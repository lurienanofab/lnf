using System;
using System.Collections.Generic;

namespace LNF.Data
{
    public interface IDataFeed
    {
        bool Active { get; set; }
        string DefaultParameters { get; set; }
        bool Deleted { get; set; }
        string FeedAlias { get; set; }
        string FeedDescription { get; set; }
        Guid FeedGUID { get; set; }
        int FeedID { get; set; }
        string FeedLink { get; set; }
        string FeedName { get; set; }
        string FeedQuery { get; set; }
        DataFeedType FeedType { get; set; }
        bool Private { get; set; }

        /// <summary>
        /// Gets a dictionary that contains default parameters (if any) merged with the given parameters, overwritting defaults.
        /// </summary>
        void ApplyDefaultParameters(IDictionary<object, object> parameters);
    }
}