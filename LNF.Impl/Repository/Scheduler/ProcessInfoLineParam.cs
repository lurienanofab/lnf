using LNF.DataAccess;

namespace LNF.Impl.Repository.Scheduler
{
    public class ProcessInfoLineParam : IDataItem
    {
        public virtual int ProcessInfoLineParamID { get; set; }
        public virtual Resource Resource { get; set; }
        public virtual string ParameterName { get; set; }
        public virtual int ParameterType { get; set; }
    }
}
