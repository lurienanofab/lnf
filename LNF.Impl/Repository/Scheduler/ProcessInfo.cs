using LNF.DataAccess;

namespace LNF.Impl.Repository.Scheduler
{
    public class ProcessInfo : IDataItem
    {
        public virtual int ProcessInfoID { get; set; }
        public virtual Resource Resource { get; set; }
        //public virtual int ProcessInfoUnitID { get; set; }
        public virtual string ProcessInfoName { get; set; }
        public virtual string ParamName { get; set; }
        public virtual string ValueName { get; set; }
        public virtual string Special { get; set; }
        public virtual bool AllowNone { get; set; }
        public virtual int Order { get; set; }
        public virtual bool RequireValue { get; set; }
        public virtual bool RequireSelection { get; set; }
    }
}
