using LNF.Control;
using LNF.DataAccess;

namespace LNF.Impl.Repository.Control
{
    public class PointInfo : IPoint, IDataItem
    {
        public virtual int Index { get; set; }
        public virtual int BlockID { get; set; }
        public virtual string BlockName { get; set; }
        public virtual string IPAddress { get; set; }
        public virtual int PointID { get; set; }
        public virtual int ModPosition { get; set; }
        public virtual int Offset { get; set; }        
        public virtual string InstanceName { get; set; }
        public virtual int ActionID { get; set; }
        public virtual string ActionName { get; set; }
    }
}

