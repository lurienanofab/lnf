using LNF.Control;
using LNF.DataAccess;

namespace LNF.Impl.Repository.Control
{
    public class ActionInstance : IActionInstance, IDataItem
    {
        public virtual int Index { get; set; }
        public virtual int Point { get; set; }
        public virtual int ActionID { get; set; }
        public virtual string Name { get; set; }
        public virtual string ActionName { get; set; }
    }
}
