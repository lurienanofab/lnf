using System.Collections.Specialized;

namespace LNF.Reporting
{
    public class UserCriteria : DefaultCriteria
    {
        public int ClientID { get; set; }

        public UserCriteria(IProvider provider) : base(provider) { }

        public UserCriteria(IProvider provider, NameValueCollection nvc) : base(provider, nvc) { }
    }
}
