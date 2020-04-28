using LNF.DataAccess;

namespace LNF.Impl.Repository.Control
{
    public class Property : IDataItem
    {
        public virtual int PropertyID { get; set; }
        public virtual string PropertyName { get; set; }
        public virtual string PropertyValue { get; set; }
    }
}
