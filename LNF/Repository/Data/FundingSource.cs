namespace LNF.Repository.Data
{
    public class FundingSource : IDataItem
    {
        public virtual int FundingSourceID { get; set; }
        public virtual string FundingSourceName { get; set; }
    }
}
