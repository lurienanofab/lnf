using LNF.DataAccess;

namespace LNF.Impl.Repository.Scheduler
{
    public class LogPropertyValue : IDataItem
    {
        public virtual int LogPropertyValueID { get; set; }
        public virtual LogProperty LogProperty { get; set; }
        public virtual string Text { get; set; }
        public virtual string Value { get; set; }
    }
}