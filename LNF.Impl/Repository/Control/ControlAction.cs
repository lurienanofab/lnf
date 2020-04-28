using LNF.Control;
using LNF.DataAccess;

namespace LNF.Impl.Repository.Control
{
    public class ControlAction : IControlAction, IDataItem
    {
        public virtual int ActionID { get; set; }
        public virtual string ActionName { get; set; }
        public virtual string ActionTableName { get; set; }
    }
}
