using LNF.DataAccess;

namespace LNF.Impl.Repository.Data
{
    public class ChargeType : IDataItem
    {
        public virtual int ChargeTypeID { get; set; }
        public virtual string ChargeTypeName { get; set; }
        public virtual int AccountID { get; set; }
        public virtual bool IsInternal { get; set; }
    }
}
