using LNF.DataAccess;

namespace LNF.Impl.Repository.Control
{
    public class Point : IDataItem
    {
        public virtual int PointID { get; set; }
        public virtual Block Block { get; set; }
        public virtual int ModPosition { get; set; }
        public virtual int Offset { get; set; }
        public virtual string Name { get; set; }
    }
}
