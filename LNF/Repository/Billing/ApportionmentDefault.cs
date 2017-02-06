using LNF.Repository.Data;

namespace LNF.Repository.Billing
{
    public class ApportionmentDefault : IDataItem
    {
        public virtual int ApportionmentDefaultID { get; set; }
        public virtual Client Client { get; set; }
        public virtual Room Room { get; set; }
        public virtual Account Account { get; set; }
        public virtual decimal Percentage { get; set; }
    }
}
