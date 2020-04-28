using LNF.DataAccess;

namespace LNF.Impl.Repository.Data
{
    public class OrgType : IDataItem
    {
        public virtual int OrgTypeID { get; set; }
        public virtual ChargeType ChargeType { get; set; }
        public virtual string OrgTypeName { get; set; }
    }
}
