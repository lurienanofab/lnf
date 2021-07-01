using LNF.DataAccess;
using LNF.Scheduler;

namespace LNF.Impl.Repository.Scheduler
{
    public class ProcessInfo : IDataItem, IProcessInfo
    {
        public virtual int ProcessInfoID { get; set; }
        public virtual int ResourceID { get; set; }
        public virtual string ProcessInfoName { get; set; }
        public virtual string ParamName { get; set; }
        public virtual string ValueName { get; set; }
        public virtual string Special { get; set; }
        public virtual bool AllowNone { get; set; }
        public virtual int Order { get; set; }
        public virtual bool RequireValue { get; set; }
        public virtual bool RequireSelection { get; set; }
        public virtual int MaxAllowed { get; set; }
        public virtual bool Deleted { get; set; }

        public override string ToString()
        {
            return $"{ProcessInfoName} [{ProcessInfoID}]";
        }
    }
}
