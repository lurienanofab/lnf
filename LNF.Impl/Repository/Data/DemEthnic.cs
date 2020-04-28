using LNF.DataAccess;

namespace LNF.Impl.Repository.Data
{
    public class DemEthnic : IDataItem
    {
        public virtual int DemEthnicID { get; set; }
        public virtual string DemEthnicValue { get; set; }
    }
}
