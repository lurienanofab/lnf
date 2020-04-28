using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Data
{
    public class Script: IDataItem
    {
        public virtual int ScriptID { get; set; }
        public virtual string ScriptName { get; set; }
        public virtual string ScriptText { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual DateTime ModifiedOn { get; set; }
        public virtual Client CreatedBy { get; set; }
        public virtual Client ModifiedBy { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
