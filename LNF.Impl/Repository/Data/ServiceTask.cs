using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Data
{
    public class ServiceTask : IDataItem
    {
        public virtual int ServiceTaskID { get; set; }
        public virtual Guid MessageID { get; set; }
        public virtual string TaskName { get; set; }
        public virtual DateTime StartTime { get; set; }
        public virtual DateTime? EndTime { get; set; }
        public virtual string Options { get; set; }
        public virtual string ErrorMessage { get; set; }
    }
}
