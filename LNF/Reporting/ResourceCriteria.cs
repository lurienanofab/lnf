using System.Collections.Specialized;

namespace LNF.Reporting
{
    public class ResourceCriteria : DefaultCriteria
    {
        public int ResourceID { get; set; }

        public ResourceCriteria(IProvider provider) : base(provider) { }

        public ResourceCriteria(IProvider provider, params NameValueCollection[] nvc) : base(provider, nvc) { }
    }
}
