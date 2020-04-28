using LNF.Billing;
using LNF.DataAccess;

namespace LNF.Impl.Repository.Billing
{
    public class ApportionmentClient : IApportionmentClient, IDataItem
    {
        public virtual int ClientID { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Emails { get; set; }
        public virtual int AccountCount { get; set; }
    }
}
