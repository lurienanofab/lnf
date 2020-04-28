using LNF.DataAccess;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Repository.Billing
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
