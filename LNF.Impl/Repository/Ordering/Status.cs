using LNF.DataAccess;

namespace LNF.Impl.Repository.Ordering
{
    public class Status : IDataItem
    {
        public virtual int StatusID { get; set; }
        public virtual string StatusName { get; set; }
    }
}
