using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace LNF.Reporting
{
    public class UserCriteria : DefaultCriteria
    {
        public int ClientID { get; set; }

        public UserCriteria() { }

        public UserCriteria(NameValueCollection nvc)
            : base(nvc) { }
    }
}
