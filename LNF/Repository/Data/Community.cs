using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Data
{
    public class Community : IDataItem
    {
        public virtual int CommunityID { get; set; }
        public virtual int CommunityFlag { get; set; }
        public virtual string CommunityName { get; set; }
    }
}
