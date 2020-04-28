using LNF.DataAccess;

namespace LNF.Impl.Repository.Data
{
    public class FundingSource : IDataItem
    {
        public virtual int FundingSourceID { get; set; }
        public virtual string FundingSourceName { get; set; }
    }
}
