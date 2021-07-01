using LNF.DataAccess;
using LNF.Scheduler;

namespace LNF.Impl.Repository.Scheduler
{
    public class ProcessInfoLineParam : IDataItem, IProcessInfoLineParam
    {
        public virtual int ProcessInfoLineParamID { get; set; }
        public virtual int ResourceID { get; set; }
        public virtual string ParameterName { get; set; }
        public virtual string ParameterUnit { get; set; }
        public virtual int ParameterType { get; set; }
    }
}
