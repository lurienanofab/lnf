using LNF.DataAccess;

namespace LNF.Impl.Repository.Scheduler
{
    public class LogProperty : IDataItem
    {
        public virtual int LogPropertyID { get; set; }
        public virtual string PropertyName { get; set; }
    }
}