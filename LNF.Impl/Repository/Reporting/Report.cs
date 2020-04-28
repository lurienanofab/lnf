using LNF.DataAccess;

namespace LNF.Impl.Repository.Reporting
{
    public class Report : IDataItem
    {
        public virtual int ReportID { get; set; }
        public virtual ReportCategory Category { get; set; }
        public virtual string Slug { get; set; }
        public virtual string Name { get; set; }
        public virtual string FeedAlias { get; set; }
        public virtual bool Active { get; set; }
    }
}
