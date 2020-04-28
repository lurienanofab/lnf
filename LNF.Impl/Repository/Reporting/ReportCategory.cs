using LNF.DataAccess;

namespace LNF.Impl.Repository.Reporting
{
    public class ReportCategory : IDataItem
    {
        public virtual int CategoryID { get; set; }
        public virtual string Slug { get; set; }
        public virtual string Name { get; set; }
        public virtual bool Active { get; set; }
    }
}
