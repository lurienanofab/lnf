using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace LNF.Reporting
{
    public class ResourceCriteria : DefaultCriteria
    {
        public int ResourceID { get; set; }

        public ResourceCriteria() { }

        public ResourceCriteria(params NameValueCollection[] nvc)
            : base(nvc) { }
    }
}
